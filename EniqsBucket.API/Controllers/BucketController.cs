using System.Threading.Tasks;
using EniqsBucket.Core.Communications.Bucket;
using EniqsBucket.Core.Communications.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EniqsBucket.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class BucketController:Controller
    {
        private readonly IBucketRepository _bucketRepository;
        private readonly ILogger<BucketController> _logger;

        public BucketController(IBucketRepository bucketRepository,ILogger<BucketController> logger)
        {
            _bucketRepository = bucketRepository;
            _logger = logger;
        }


        [HttpPost("{bucketName}")]
        public async Task<ActionResult<CreateBucketResponse>> CreateBucketAsync(string bucketName)
        {
            _logger.LogInformation("Incoming Create Bucket request");

            if (await _bucketRepository.DoesS3BucketExist(bucketName)) return BadRequest("Bucket already exists!");
            var result = await _bucketRepository.CreateS3BucketAsync(bucketName);
            if (result != null && !string.IsNullOrEmpty(result.RequestId)) return Ok(result);

            _logger.LogInformation("Something went wrong along the way");
            return BadRequest("Unable to create S3 bucket");
        }

        [HttpGet]
        public async Task<ActionResult<ListS3BucketResponse>> GetBucketsAsync() {
            _logger.LogInformation("Incoming Get All Buckets request");

            var result = await _bucketRepository.GetAllBucketsAsync();
            if (result == null) return NotFound();

            return Ok(result);
        }
        [HttpDelete("{bucketName}")]
        public async Task<IActionResult> DeleteBucketsAsync(string bucketName)
        {
            _logger.LogInformation("Incoming Delete Buckets request");

            var result = await _bucketRepository.DeleteBucketAsync(bucketName);
            if (result) return NoContent();

            return BadRequest("Unable to delete bucket at this time");
        }
     
    }
}
