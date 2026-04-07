using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.Payment
{
    public interface IPaymentService
    {
        Task<GenericResponse> GeneratePaymentLink(GeneratePaymentLinkDto request);
        Task<GenericResponse> VerifyPayment(string transactionRef);
    }
}
