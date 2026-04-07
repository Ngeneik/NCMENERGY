using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.MailService
{
    public interface IMailService
    {
        Task<GenericResponse> SendEmail(MailDto request);

    }
}
