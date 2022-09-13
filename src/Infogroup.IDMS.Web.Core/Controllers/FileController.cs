using System;
using System.Net;
using System.Threading.Tasks;
using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;
using Infogroup.IDMS.Dto;
using Infogroup.IDMS.Storage;
using System.IO;
using System.Net.Http.Headers;
using Infogroup.IDMS.IDMSConfigurations;
using Infogroup.IDMS.Databases;
using System.Linq;
using Infogroup.IDMS.Divisions;
using Abp.Domain.Repositories;
using System.Text.RegularExpressions;
using Abp.UI;
using Infogroup.IDMS.Common;

namespace Infogroup.IDMS.Web.Controllers
{
    public class FileController : IDMSControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IRedisIDMSConfigurationCache _idmsConfigurationCache;
        private readonly IDatabaseRepository _customDatabaseRepository;
        private readonly IRepository<Database, int> _databaseRepository;
        private readonly IRepository<Division, int> _divisionRepository;
        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            IRedisIDMSConfigurationCache idmsConfigurationCache,
            IDatabaseRepository customDatabaseRepository,
            IRepository<Database, int> databaseRepository,
            IRepository<Division, int> divisionRepository
        )
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _idmsConfigurationCache = idmsConfigurationCache;
            _customDatabaseRepository = customDatabaseRepository;
            _databaseRepository = databaseRepository;
            _divisionRepository = divisionRepository;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }

            return File(fileBytes, file.FileType, file.FileName);
        }

        [DisableAuditing]
        public ActionResult DownloadFile(FileDto file)
        {
            var fileBytes = System.IO.File.ReadAllBytes(file.FileName);
            var filename = Path.GetFileName(file.FileName);

            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }
            if (file.ItShouldDelete)
                System.IO.File.Delete(file.FileName);
            return File(fileBytes, file.FileType, filename);
        }

        public async Task<ActionResult> DownloadFileDocumentAttchment(FileDto file)
        {
            byte[] fileBytes;
            if (file.IsAWS)
            {
                try
                {
                    var s3Util = new S3Utilities();
                    fileBytes = await s3Util.DownloadFileAsync(file.FileName);
                }
                catch (Exception)
                {
                    return NotFound(L("RequestedFileDoesNotExists"));
                }
            }
            else
            {
                fileBytes = System.IO.File.ReadAllBytes(file.FileName);
                if (file.ItShouldDelete)
                    System.IO.File.Delete(file.FileName);
            }

            if (fileBytes == null)
                return NotFound(L("RequestedFileDoesNotExists"));

            var filename = file.DownloadedFileName;
            return File(fileBytes, file.FileType, filename);
        }

        [DisableAuditing]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType, string fileName)
        {
            var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (fileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return File(fileObject.Bytes, contentType, fileName);
        }
        public ActionResult<string> ValidateSuppressionFile(int campaignId, string fileName, string cFieldName)
        {
            try
            {

                var databaseID = _customDatabaseRepository.GetDataSetDatabaseByOrderID(campaignId).Id;
                var cPath = _idmsConfigurationCache.GetConfigurationValue("LargeSuppressionPath", databaseID)?.cValue;
                //var cPath = @"D:\Dump";
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);
                var extension = Path.GetExtension(fileName);
                var fullPath = Path.Combine(cPath, fileName);

                if (fileName.Contains("'"))
                    return StatusCode(403, Json("invalid_filename_quotes"));
                if (!Regex.IsMatch(fileName, @"^[\w\-. ]+$"))
                    return StatusCode(403, Json("invalid_file_name"));
                if (!Regex.IsMatch(fileName, @"[^\\]*\.(\w+)$"))
                    return StatusCode(403, Json("invalid_file_name"));
                if (!CheckFileExtension(extension, awsFlag))
                {
                    if (awsFlag)
                        return StatusCode(403, Json("invalid_file_typeS3"));
                    else
                        return StatusCode(403, Json("invalid_file_type"));
                }
                if (extension.ToUpper() == ".ZIP" && cFieldName.ToUpper().Equals("ZIPRADIUS"))
                    return StatusCode(403, Json("invalid_zip_extension"));
                //if (!System.IO.File.Exists(fullPath))
                //    return StatusCode(403, Json("file_not_exist"));

                return fullPath;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult<string> UploadFile(int campaignId)
        {
            try
            {
                var databaseID = _customDatabaseRepository.GetDataSetDatabaseByOrderID(campaignId).Id;
                var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);
                var cPath = string.Empty;
                if (awsFlag)
                    cPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSybase", databaseID)?.cValue;
                else
                    cPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", databaseID)?.cValue;
                //var cPath = @"D:\Test\";
                var file = Request.Form.Files[0];
                //var cPath = @"D:\Test\";
                //var cPath = @"s3://axle-raw-customer-sources-dev/Test/";
                var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var extension = Path.GetExtension(fileName);
                var sFileName = DateTime.Now.Ticks + "_" + fileName;

                if (file.Length > 4294967290)
                {
                    return StatusCode(403, Json("file_exceed_max"));
                }
                else
                {
                    if (fileName.Contains("'"))
                        return StatusCode(403, Json("invalid_filename_quotes"));
                    if (!CheckFileExtension(extension, awsFlag))
                    {
                        if (awsFlag)
                            return StatusCode(403, Json("invalid_file_typeS3"));
                        else
                            return StatusCode(403, Json("invalid_file_type"));
                    }

                    // Check 'AWS' flag, if "1" upload file to AWS bucket else to n/w folder                    
                    if (awsFlag)
                    {
                        // upload file to aws
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            if (extension.ToUpper() != ".GZ" && !ValidateCRLF(ms, awsFlag))
                                return StatusCode(403, Json("missing_CR+LF"));
                            var bucketName = cPath.Substring(cPath.IndexOf("//") == -1 ? cPath.IndexOf(cPath) : cPath.IndexOf("//") + 2).TrimEnd('/');
                            var uploadToS3 = new S3Utilities();
                            ms.Seek(0, SeekOrigin.Begin);
                            uploadToS3.FileUploadToS3(ms, bucketName, sFileName);
                        }
                    }
                    else
                    {
                        var webRootPath = cPath;
                        if (!Directory.Exists(webRootPath))
                        {
                            Directory.CreateDirectory(webRootPath);
                        }
                        string sName = Path.Combine(webRootPath, sFileName);
                        using (var stream = new FileStream(sName, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        if (extension.ToUpper() != ".ZIP" && !ValidateCRLF(sName))
                        {
                            return StatusCode(403, Json("missing_CR+LF"));
                        }
                    }
                }
                return sFileName;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult UploadAttachmentFile(int campaignId)
        {
            var databaseID = _customDatabaseRepository.GetDatabaseIDByOrderID(campaignId).Id;
            var cPath = _idmsConfigurationCache.GetConfigurationValue("ORDER_ATTACHMENT_PATH", databaseID)?.cValue;

            var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseID);

            var file = Request.Form.Files[0];
            var code = Request.Form.First().Value.ToString();
            var cFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var cFileExtension = Path.GetExtension(cFileName);

            string realFileName = Guid.NewGuid() + cFileExtension;//".pdf";
            if (awsFlag)
            {
                // upload file to aws
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var bucketName = cPath.Substring(cPath.IndexOf("//") == -1 ? cPath.IndexOf(cPath) : cPath.IndexOf("//") + 2).TrimEnd('/');
                    var uploadToS3 = new S3Utilities();
                    ms.Seek(0, SeekOrigin.Begin);
                    uploadToS3.FileUploadToS3(ms, bucketName, realFileName);
                }
            }
            else
            {
                cPath += @"\";
                var path = cPath + realFileName;
                if (!Directory.Exists(cPath))
                {
                    Directory.CreateDirectory(cPath);
                }
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return Json(new { cFileName, realFileName, code });
        }

        public JsonResult UploadAttachmentScoreFile(int databaseId)
        {
            var cPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", databaseId)?.cValue + @"\";

            var file = Request.Form.Files[0];
            var cFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            if (!Directory.Exists(cPath))
            {
                Directory.CreateDirectory(cPath);
            }

            var fileName = $"Model_{Guid.NewGuid()}.txt";
            var path = $"{cPath}{fileName}";
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return Json(new { cFileName, fileName });
        }

        public JsonResult UploadAttachmentMatchAppend(int databaseId)
        {
            var cReadyToLoadFilesPath = string.Empty;
            var awsFlag = _idmsConfigurationCache.IsAWSConfigured(databaseId);
            var cPath = _idmsConfigurationCache.GetConfigurationValue("FileUploadPath", databaseId)?.cValue;
            cPath += awsFlag ? string.Empty : @"\";

            if (string.IsNullOrEmpty(cPath))
            {
                var message = L("FilePathNotConfiguredValidation");

                return Json(new { IsCreated = false, message });
            }
            var file = Request.Form.Files[0];
            if(awsFlag && file.FileName.ToLower().EndsWith(".xlsx"))
            {
                var message = L("ExcelFeatureNotSupported");
                return Json(new { IsCreated = false, message });
            }              

            if (!CheckContents(file.ContentType.ToLower().Trim(), file.FileName))
            {
                var message = L("IncorrectFileFormatValidation");
                var fileName = file.FileName;
                return Json(new { IsCreated = false, message, fileName });
            }
            cReadyToLoadFilesPath = cPath;
            var LoadFilePath = (cReadyToLoadFilesPath).Replace("|", "");
            var sClientFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var path = $"{LoadFilePath}{file.FileName}";
            if (awsFlag)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var bucketName = LoadFilePath.Substring(LoadFilePath.IndexOf("//") == -1 ? LoadFilePath.IndexOf(LoadFilePath) : LoadFilePath.IndexOf("//") + 2).TrimEnd('/');
                    var uploadToS3 = new S3Utilities();
                    ms.Seek(0, SeekOrigin.Begin);
                    uploadToS3.FileUploadToS3(ms, bucketName, sClientFileName);
                }
            }
            else
            {
                if (!Directory.Exists(LoadFilePath))
                {
                    Directory.CreateDirectory(LoadFilePath);
                }                
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            return Json(new { IsCreated = true, sClientFileName, path });
        }

        private bool CheckContents(string sFileContent, string filename)
        {
            //excel and zip is not allowed.
            if ((sFileContent == "text/plain") || (sFileContent == "text/csv") || (sFileContent == "text/comma-separated-values") || (filename.ToLower().EndsWith(".csv")) || (filename.ToLower().EndsWith(".xlsx")))
            {

                return true;
            }

            return false;
        }

        public JsonResult UploadAttachedFile(int databaseId)
        {
            try
            {
                var divisionId = _databaseRepository.GetAll().FirstOrDefault(p => p.Id == databaseId).DivisionId;
                var cPath = _divisionRepository.GetAll().FirstOrDefault(p => p.Id == divisionId).cOfferPath;

                var file = Request.Form.Files[0];
                var sClientFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                if (!Directory.Exists(cPath.ToString()))
                {
                    Directory.CreateDirectory(cPath.ToString());
                }

                var sFileName = $"Sample_{Guid.NewGuid()}_{sClientFileName}";
                var path = Path.Combine(cPath, sFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return Json(new { sClientFileName, path });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ValidateCRLF(Stream fileStream, bool awsFlag)
        {
            bool success = true;

            if (!awsFlag)
            {
                fileStream.Seek(-2, SeekOrigin.End);
                var bytes = new byte[2];
                fileStream.Read(bytes, 0, 2);
                //fileStream.Close();

                if (Convert.ToChar(bytes[0]) != (char)13 || Convert.ToChar(bytes[1]) != (char)10)
                    success = false;
            }
            else
            {
                fileStream.Seek(-1, SeekOrigin.End);
                var bytes = new byte[1];
                fileStream.Read(bytes, 0, 1);

                if (Convert.ToChar(bytes[0]) != (char)13 && Convert.ToChar(bytes[0]) != (char)10)
                    success = false;
            }
            return success;
        }

        private bool ValidateCRLF(string fileName)
        {
            bool success = true;

            var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            fs.Seek(-2, SeekOrigin.End);
            var bytes = new byte[2];
            fs.Read(bytes, 0, 2);
            fs.Close();

            if (Convert.ToChar(bytes[0]) != (char)13 || Convert.ToChar(bytes[1]) != (char)10)
                success = false;

            return success;
        }

        private bool CheckFileExtension(string sFileType, bool awsFlag)
        {
            string sFileExtension = sFileType;

            switch (sFileExtension.ToUpper())
            {
                case ".TXT":
                case ".DAT":
                case ".CSV":
                    sFileExtension = ".TXT";
                    break;
                case ".ZIP":
                case ".GZ":
                    sFileExtension = awsFlag && sFileExtension.ToUpper() == ".GZ" ? ".GZ" : (!awsFlag && sFileExtension.ToUpper() == ".ZIP" ? ".ZIP" : string.Empty);
                    break;
                default:
                    sFileExtension = string.Empty;
                    break;
            }
            sFileType = sFileExtension;

            bool bReturn = string.IsNullOrEmpty(sFileType) ? false : true;
            return bReturn;
        }

        public JsonResult UploadBulkSegment(int databaseId)
        {
            try
            {
                var message = string.Empty;
                var sPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", databaseId)?.cValue + @"\";

                var file = Request.Form.Files[0];
                var sFileName = DateTime.Now.Ticks + "_" + ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                if (string.IsNullOrEmpty(sPath))
                    message = L("FileConfigureMsg");
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                var path = Path.Combine(sPath, sFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    file.CopyTo(stream);

                return Json(new { path, message });
            }
            catch (Exception ex)
            {
                return Json(new { ex.Message });
            }
        }
        public JsonResult UploadLayout(int databaseId)
        {
            try
            {
                var message = string.Empty;
                var dateTimeHashCode = DateTime.Now.GetHashCode().ToString();
                dateTimeHashCode = dateTimeHashCode.Substring(dateTimeHashCode.Length - 4);
                var sPath = _idmsConfigurationCache.GetConfigurationValue("SelectionFilesSQL", databaseId)?.cValue + @"\";
                var file = Request.Form.Files[0];
                var clientFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                var ext = Path.GetExtension(clientFileName);
                var name = Path.GetFileNameWithoutExtension(clientFileName);
                var sFileName = $"ExportLayout_{name}_{dateTimeHashCode}{ext}";
                if (string.IsNullOrEmpty(sPath))
                    message = L("FileConfigureMsg");
                if (!Directory.Exists(sPath))
                    Directory.CreateDirectory(sPath);

                var path = Path.Combine(sPath, sFileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    file.CopyTo(stream);

                return Json(new { path, message });
            }
            catch (Exception ex)
            {
                return Json(new { ex.Message });
            }
        }
    }

}