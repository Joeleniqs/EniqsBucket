using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using EniqsBucket.Core.Communications.File;
using EniqsBucket.Core.Communications.Interfaces;
using Microsoft.AspNetCore.Http;
using static EniqsBucket.Core.enums;

namespace EniqsBucket.Infrastructure.Repositories
{

    public class FilesRepository : IFilesRepository
    {
        private readonly IAmazonS3 _s3Client;

        public FilesRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<IEnumerable<AddFileResponse>> AddFilesAsync(string bucketName, IList<IFormFile> formFiles,FileEnum fileType)
        {
            var response = new List<AddFileResponse>();

            foreach (var file in formFiles)
            {
                var fileName = "";
                S3CannedACL  acl = null;
                switch (fileType) {
                    case FileEnum.ProductImage :
                        fileName = $"Images/Products/{file.FileName}";
                        acl = S3CannedACL.PublicRead;
                        break;
                    case FileEnum.UserImage:
                        fileName = $"Images/Users/{file.FileName}";
                        acl = S3CannedACL.PublicRead;
                        break;
                    case FileEnum.UserDocument:
                        fileName = $"Documents/UserDocuments/{file.FileName}";
                        acl = S3CannedACL.NoACL;
                        break;
                    case FileEnum.IdentityDocument:
                        fileName = $"Documents/IdentityDocuments/{file.FileName}";
                        acl = S3CannedACL.NoACL;
                        break;
                    default: return null;
                }

                var requestUpload = new TransferUtilityUploadRequest
                {
                    //add custom folder structure for uniqueness
                    Key = fileName,
                    CannedACL = acl,
                    InputStream = file.OpenReadStream(),
                    BucketName = bucketName
                };
                using (var transferUtility = new TransferUtility(_s3Client)) {
                    await transferUtility.UploadAsync(requestUpload);
                };
                var expiryUrl = new GetPreSignedUrlRequest {
                    BucketName = bucketName,
                    Key = file.FileName,
                    Expires = DateTime.Now.AddDays(1)
                };
                var url = _s3Client.GetPreSignedURL(expiryUrl);
                var objectUrl = $"https://{bucketName}.s3.amazonaws.com/{fileName}";
                response.Add(new AddFileResponse {
                    preConfiguredUrl = url,
                    ActualObjectUrl = objectUrl
                });
            }

            return response;
        }

        public async Task<IEnumerable<ListFilesResponse>> GetFilesAsync(string bucketName)
        {
            var response = await _s3Client.ListObjectsAsync(bucketName);

            return response.S3Objects.Select(p => new ListFilesResponse
            {
                Key = p.Key,
                BucketName = p.BucketName,
                Owner = p.Owner.DisplayName,
                Size = p.Size
            });
        }

        public async Task DownloadAsync(string bucketName , string fileName) {
            var downloadPath =   $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}/Downloads/s3Temp/{fileName}";
            var request = new TransferUtilityDownloadRequest {
                BucketName = bucketName,
                Key = fileName,
                FilePath = downloadPath,
            };
            using (var transferUtility = new TransferUtility(_s3Client)) {
                await transferUtility.DownloadAsync(request);
            }
        }

        public async Task<DeleteFileResponse> DeleteAsync(string bucketName, string fileName)
        {
            var multipleDeleteObjectRequest = new DeleteObjectsRequest() { BucketName = bucketName };
            multipleDeleteObjectRequest.AddKey(fileName);
            var response = await _s3Client.DeleteObjectsAsync(multipleDeleteObjectRequest);
            return new DeleteFileResponse {
                NumberOfDeletedObjects = response.DeletedObjects.Count
            };
        }


    }
}
