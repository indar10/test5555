using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Amazon.S3.Util;
using System.Collections.Generic;

namespace Infogroup.IDMS.Common
{
    public class S3Utilities
    {
        private AmazonS3Client client { get; set; }
        public S3Utilities()
        {
            client = LoadConfiguration();
        }
        private AmazonS3Client LoadConfiguration()
        {
            var local_creds = File.ReadAllLines(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "default_aws_profile.txt"));
            var key = local_creds[0];
            var secret = local_creds[1];
            var region = RegionEndpoint.GetBySystemName(local_creds[2]);
            var creds = new Amazon.Runtime.BasicAWSCredentials(key, secret);
            return new AmazonS3Client(creds, region);
        }
        public void FileUploadToS3(Stream localFilePath, string bucketName, string fileNameInS3)
        {
            TransferUtility utility = new TransferUtility(client);
            TransferUtilityUploadRequest request = new TransferUtilityUploadRequest
            {
                BucketName = bucketName,
                Key = fileNameInS3,
                PartSize = 6291456, // 6 MB.
                InputStream = localFilePath,
                //ServerSideEncryptionMethod = ServerSideEncryptionMethod.AWSKMS
            };
            utility.Upload(request);
        }
        
        public async Task<byte[]> DownloadFileAsync(string filePath)
        {
            try
            {
                (string bucket, string key, string fileName) = S3Parser.TryParseS3Uri(filePath);
                var dest = Path.Combine(Path.GetTempPath(), fileName);
                var request = new GetObjectRequest
                {
                    BucketName = bucket,
                    Key = key
                };
                using (GetObjectResponse response = await client.GetObjectAsync(request))
                {
                    await response.WriteResponseStreamToFileAsync(dest, false, default);
                    var fileInBytes = File.ReadAllBytes(dest);
                    File.Delete(dest);
                    return fileInBytes;
                }
            }
            catch (AmazonS3Exception e)
            {
                // If bucket or object does not exist
                throw;
            }
            catch (Exception e)
            {
                throw;               
            }
        }

        public async Task CopyObject(string sourceBucket, string objectKey, string destinationBucket, string destObjectKey)
        {
            try
            {
                Amazon.S3.Model.CopyObjectRequest request = new Amazon.S3.Model.CopyObjectRequest
                {
                    SourceBucket = sourceBucket,
                    SourceKey = objectKey,
                    DestinationBucket = destinationBucket,
                    DestinationKey = destObjectKey
                };
                await client.CopyObjectAsync(request);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Amazon.S3.Model.S3Object> ListFilesFromUri(string urlString)
        {
            var uri = new S3Path(urlString);
            List<Amazon.S3.Model.S3Object> bucketFiles = new List<Amazon.S3.Model.S3Object>();
            ListObjectsV2Response response;

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = uri.BucketName,
                MaxKeys = 500,
                Prefix = uri.Key
            };

            while (true)
            {
                response = client.ListObjectsV2Async(request).Result;
                bucketFiles.AddRange(response.S3Objects);

                if (response.IsTruncated)
                    request.ContinuationToken = response.NextContinuationToken;
                else
                    break;
            }

            return bucketFiles;
        }

        public async Task MoveObject(string sourceBucket, string objectKey, string destinationBucket, string destObjectKey)
        {
            try
            {
                CopyObjectRequest crequest = new CopyObjectRequest
                {
                    SourceBucket = sourceBucket,
                    SourceKey = objectKey,
                    DestinationBucket = destinationBucket,
                    DestinationKey = destObjectKey
                };
                await client.CopyObjectAsync(crequest);

                DeleteObjectRequest drequest = new DeleteObjectRequest
                {
                    BucketName = sourceBucket,
                    Key = objectKey
                };
                await client.DeleteObjectAsync(drequest);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //public void FileDownloadFromS3(string filename, string bucketName)
        //{
        //    try
        //    {
        //        string[] keySplit = filename.Split('/');
        //        string fileName = keySplit[keySplit.Length - 1];
        //        string dest = Path.Combine(HttpRuntime.CodegenDir, fileName);
        //        using (client)
        //        {
        //            GetObjectRequest request = new GetObjectRequest
        //            {
        //                BucketName = bucketName,
        //                Key = filename
        //            };
        //            using (GetObjectResponse response = client.GetObject(request))
        //            {
        //                response.WriteResponseStreamToFile(dest, false);
        //            }
        //            HttpContext.Current.Response.Clear();
        //            HttpContext.Current.Response.AppendHeader("content-disposition", "attachment; filename=" + fileName);
        //            HttpContext.Current.Response.ContentType = "application/octet-stream";
        //            HttpContext.Current.Response.TransmitFile(dest);
        //            HttpContext.Current.Response.Flush();
        //            HttpContext.Current.Response.End();
        //            // Clean up temporary file.
        //            File.Delete(dest);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }

        //}
    }
    
    public static class S3Parser
    {
        public static Tuple<string, string,string> TryParseS3Uri(string x)
        {
            try
            {
                var uri = new Uri(x);

                if (uri.Scheme == "s3")
                {
                    var bucket = uri.Host;
                    var key = uri.LocalPath.Substring(1);
                    var fileName = uri.Segments[uri.Segments.Length - 1];
                    return new Tuple<string, string, string>(bucket, key, fileName);
                }

                return null;
            }
            catch (Exception ex)
            {
                var ex2 = ex as UriFormatException;

                if (ex2 == null)
                {
                    throw ex;
                }

                return null;
            }
        }
    }
}
