using System.Collections.Generic;
using System.Threading.Tasks;
using EniqsBucket.Core.Communications.File;
using Microsoft.AspNetCore.Http;
using static EniqsBucket.Core.enums;

namespace EniqsBucket.Core.Communications.Interfaces
{

    public interface IFilesRepository
    {
        public Task<IEnumerable<AddFileResponse>> AddFilesAsync(string bucketName, IList<IFormFile> formFiles, FileEnum fileType);
        public Task<IEnumerable<ListFilesResponse>> GetFilesAsync(string bucketName);
        public Task DownloadAsync(string bucketName,string fileName);
        public Task<DeleteFileResponse> DeleteAsync(string bucketName, string fileName);
    }
}
