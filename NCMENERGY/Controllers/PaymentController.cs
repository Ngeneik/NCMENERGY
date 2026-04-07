using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Dtos;
using NCMENERGY.Services; // adjust namespace if needed
using NCMENERGY.Services.Payment;
using System;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("generate-link")]
        public async Task<IActionResult> GeneratePaymentLink([FromBody] GeneratePaymentLinkDto request)
        {
            var response = await _paymentService.GeneratePaymentLink(request);
            return StatusCode(response.Success ? 200 : 400, response);
        }

        [HttpGet("verify/{transactionRef}")]
        public async Task<IActionResult> VerifyPayment(string transactionRef)
        {
            var response = await _paymentService.VerifyPayment(transactionRef);
            return StatusCode(response.Success ? 200 : 400, response);
        }
    }
}