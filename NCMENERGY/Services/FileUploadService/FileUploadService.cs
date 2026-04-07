using NCMENERGY.Dtos;
using NCMENERGY.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace NCMENERGY.Services.FileUploadService
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUploadService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GenericResponse> UploadFile(FileUploadDto request)
        {
            if (request?.Files == null || !request.Files.Any())
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "No files provided."
                };
            }

            var fileUrls = new List<string>();

            // fallback if WebRootPath is null
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in request.Files)
            {
                if (file == null || file.Length == 0) continue;

                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                var requestContext = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = requestContext != null
                    ? $"{requestContext.Scheme}://{requestContext.Host}"
                    : "http://localhost"; // fallback

                var fileUrl = $"{baseUrl}/uploads/{fileName}";
                fileUrls.Add(fileUrl);
            }

            return new GenericResponse
            {
                Success = true,
                Data = fileUrls
            };
        }
    }
}