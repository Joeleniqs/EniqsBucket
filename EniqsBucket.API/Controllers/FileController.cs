using System.Collections.Generic;
using System.Threading.Tasks;
using EniqsBucket.Core.Communications.File;
using EniqsBucket.Core.Communications.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static EniqsBucket.Core.enums;

namespace EniqsBucket.API.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class FileController : Controller
    {
        private readonly IFilesRepository _filesRepo;

        public FileController(IFilesRepository filesRepository)
        {
            _filesRepo = filesRepository;
        }

        [HttpPost("{bucketName}")]
        public async Task<ActionResult<IEnumerable<AddFileResponse>>> Add(string bucketName, IList<IFormFile> formFiles,[FromForm] FileEnum fileType)
        {
            if (formFiles == null) return BadRequest("There are no provided files for upload!");
            var result = await _filesRepo.AddFilesAsync(bucketName, formFiles,fileType);
            if (result == null) return BadRequest("unable to creat upload files at this time");

            return Ok(result);
        }

        [HttpGet("{bucketName}")]
        public async Task<ActionResult<IEnumerable<ListFilesResponse>>>GetFiles(string bucketName)
        {
            var result = await _filesRepo.GetFilesAsync(bucketName);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("{bucketName}/Download/{filename}")]
        public async Task<IActionResult> DownloadFile(string bucketName, string filename) {
            await _filesRepo.DownloadAsync(bucketName, filename);
            return Ok();
        }
        [HttpDelete("{bucketName}/{filename}")]
        public async Task<IActionResult> DeleteFile(string bucketName, string filename)
        {
           var response = await _filesRepo.DeleteAsync(bucketName, filename);
            return Ok(response);
        }
    }

}
