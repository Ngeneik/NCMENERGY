using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.FileUploadService
{
    public interface IFileUploadService
    {
        Task<GenericResponse> UploadFile(FileUploadDto request);

    }
}
