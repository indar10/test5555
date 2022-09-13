using System;
using System.ComponentModel.DataAnnotations;

namespace Infogroup.IDMS.Dto
{
    public class FileDto
    {
        [Required]
        public string FileName { get; set; }

        public string FileType { get; set; }

        public bool ItShouldDelete { get; set; }

        public bool IsAWS { get; set; }

        [Required]
        public string FileToken { get; set; }

        public string DownloadedFileName { get; set; }

        public FileDto()
        {
            
        }
        public FileDto(string fileName, string fileType,bool itShouldDelete=false)
        {
            FileName = fileName;
            FileType = fileType;
            FileToken = Guid.NewGuid().ToString("N");
            ItShouldDelete = itShouldDelete;
        }
        public FileDto(string fileName, string fileType, string downloadedFileName,bool itShouldDelete = false, bool isAWS = false)
        {
            FileName = fileName;
            FileType = fileType;
            DownloadedFileName = downloadedFileName;
            FileToken = Guid.NewGuid().ToString("N");
            ItShouldDelete = itShouldDelete;
            IsAWS = isAWS;
        }
    }
}