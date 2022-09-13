using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Infogroup.IDMS.Helper
{
    public static class ZipUtil
    {
        public static void Compress(string sourcefilePath, string zipFilePath, string searchString)
        {
           
            DirectoryInfo d = new DirectoryInfo(sourcefilePath);
            string fileName = Path.GetFileNameWithoutExtension(searchString.Replace("_*", ""));
            string filePath = $"{zipFilePath}{fileName}";
            if (!Directory.Exists(zipFilePath))
            {
                Directory.CreateDirectory(zipFilePath.GetSafeFilePath());
            }
            if (File.Exists($"{filePath}.zip"))
            {
                File.Delete($"{filePath}.zip".GetSafeFilePath());
            }
            using (FileStream zipToOpen = new FileStream($"{filePath}.zip", FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    foreach (FileInfo fileToCompress in d.GetFiles(searchString, SearchOption.AllDirectories))
                    {
                        ZipArchiveEntry readmeEntry = archive.CreateEntryFromFile(fileToCompress.FullName, fileToCompress.Name);
                    }
                }
            }
          
        }
        public static string GetSafeFilePath(this string value)
        {
            return value.Replace("|", "");
        }
    }
    }
