using System.Collections.Generic;
using System.Threading.Tasks;
using EniqsBucket.Core.Communications.Bucket;

namespace EniqsBucket.Core.Communications.Interfaces
{
    public interface IBucketRepository
    {
        public  Task<bool> DoesS3BucketExist(string bucketName);
        public Task<CreateBucketResponse> CreateS3BucketAsync(string bucketName);
        public Task<IEnumerable<ListS3BucketResponse>> GetAllBucketsAsync();
        public Task<bool> DeleteBucketAsync(string bucketName);

    }
}
