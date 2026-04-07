using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Dtos;
using NCMENERGY.Services.FileUploadService;


namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] FileUploadDto request)
        {
            var result = await _fileUploadService.UploadFile(request);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}