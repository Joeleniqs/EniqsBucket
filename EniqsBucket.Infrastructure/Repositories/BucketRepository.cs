using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using EniqsBucket.Core.Communications.Bucket;
using EniqsBucket.Core.Communications.Interfaces;

namespace EniqsBucket.Infrastructure.Repositories
{
    public class BucketRepository:IBucketRepository
    {
        private readonly IAmazonS3 _s3Client;

        public BucketRepository(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public  async Task<CreateBucketResponse> CreateS3BucketAsync(string bucketName)
        {
            var putBucketRequest = new PutBucketRequest {
                BucketName = bucketName,
                UseClientRegion = true
            };
            var result = await _s3Client.PutBucketAsync(putBucketRequest);
            return new CreateBucketResponse { RequestId = result.ResponseMetadata.RequestId, BucketName = bucketName };
        }

        public async Task<bool> DoesS3BucketExist(string bucketName) =>   await _s3Client.DoesS3BucketExistAsync(bucketName);

        public async Task<IEnumerable<ListS3BucketResponse>> GetAllBucketsAsync()
        {
            var result = await _s3Client.ListBucketsAsync();
            return result.Buckets.Select(property => new ListS3BucketResponse {
                BucketName = property.BucketName,
                TimeStampCreated = property.CreationDate
            });
        }
        public async Task<bool> DeleteBucketAsync(string bucketName)
        {
            var result = await _s3Client.DeleteBucketAsync(bucketName);
            if (!string.IsNullOrEmpty(result.ResponseMetadata.RequestId)) return true ;
            return false;
        }
    }
}
