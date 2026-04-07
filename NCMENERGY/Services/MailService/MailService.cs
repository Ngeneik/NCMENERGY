using Microsoft.Extensions.Options;
using NCMENERGY.Dtos;
using NCMENERGY.Options;
using NCMENERGY.Response;
using System.Net;
using System.Net.Mail;

namespace NCMENERGY.Services.MailService
{
    public class MailService : IMailService
    {
        private readonly MailOptions _mailOptions;

        public MailService(IOptions<MailOptions> mailOptions)
        {
            _mailOptions = mailOptions.Value;
        }

        public async Task<GenericResponse> SendEmail(MailDto request)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(
                        _mailOptions.Email,
                        _mailOptions.Password
                    ),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_mailOptions.Email!),
                    Subject = request.Subject,
                    Body = request.HtmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(request.RecipientEmail);

                await smtpClient.SendMailAsync(mailMessage);

                return new GenericResponse
                {
                    Success = true,
                    Status = HttpStatusCode.OK.ToString(),
                    Message = "Email sent successfully"
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse
                {
                    Success = false,
                    Status = HttpStatusCode.InternalServerError.ToString(),
                    Message = ex.Message
                };
            }
        }

    }
}
