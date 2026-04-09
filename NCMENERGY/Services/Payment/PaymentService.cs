using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NCMENERGY.Data;
using NCMENERGY.Dtos;
using NCMENERGY.Models;
using NCMENERGY.Options;
using NCMENERGY.Response;
using NCMENERGY.Services.MailService;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;

namespace NCMENERGY.Services.Payment
{
    public class PaymentService : IPaymentService
    {
        public readonly ApplicationDbContext _context;
        private readonly PaystackIntegration _settings;
        private readonly IMailService _mailService;

        public PaymentService(ApplicationDbContext context, IOptions<PaystackIntegration> settings, IMailService mailService)
        {
            _context = context;
            _settings = settings.Value;
            _mailService = mailService;
        }
        public async Task<GenericResponse> GeneratePaymentLink(GeneratePaymentLinkDto request)
        {
            if (request.OrderSummary == null || !request.OrderSummary.Any())
                return new GenericResponse { Success = false, Message = "No items in order" };

            var productIds = request.OrderSummary.Select(o => o.ProductId).ToList();

            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            if (products.Count != productIds.Count)
                return new GenericResponse { Success = false, Message = "Some products not found" };

            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.OrderSummary)
            {
                var product = products.First(p => p.Id == item.ProductId);

                decimal amount = product.Price * item.Quantity;
                totalAmount += amount;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.ProductName,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity,
                    Amount = amount,
                    TotalAmount = amount,
                    DateCreated = DateTime.UtcNow
                });
            }

            string orderTag = $"ORD-{new Random().Next(100, 999)}";
            Guid transactionRef = Guid.NewGuid();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    Address = request.Address,
                    City = request.City,
                    State = request.State,
                    OrderTag = orderTag,
                    Status = "Pending",
                    PaymentMethod = "Paystack",
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.UtcNow,
                    OrderItems = orderItems
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var payment = new NCMENERGY.Models.Payment
                {
                    OrderId = order.Id,
                    TransactionRef = transactionRef,
                    Amount = totalAmount,
                    PaymentMethod = "Paystack",
                    Status = "Pending",
                    DateCreated = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _settings.Key);

                var paystackRequest = new
                {
                    email = request.Email,
                    amount = (int)(totalAmount * 100),
                    reference = transactionRef.ToString(),
                    callback_url = _settings.CallbackUrl
                };

                var response = await client.PostAsJsonAsync(
                    "https://api.paystack.co/transaction/initialize",
                    paystackRequest
                );

                var rawResponse = await response.Content.ReadAsStringAsync();


                var paystackResponse = await response.Content.ReadFromJsonAsync<PaystackResponse>();

                if (paystackResponse == null || !paystackResponse.Status)
                {
                    await transaction.RollbackAsync();
                    return new GenericResponse
                    {
                        Success = false,
                        Message = "Failed to create payment link"
                    };
                }

                await transaction.CommitAsync();

                return new GenericResponse
                {
                    Success = true,
                    Message = "Payment link generated successfully",
                    Data = new
                    {
                        PaymentUrl = paystackResponse.Data.AuthorizationUrl,
                        TransactionRef = transactionRef
                    }
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<GenericResponse> VerifyPayment(string transactionRef)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionRef.ToString() == transactionRef);

            if (payment == null)
                return new GenericResponse { Success = false, Message = "Payment not found" };

            // Prevent duplicate processing
            if (payment.Status == "Paid")
            {
                return new GenericResponse
                {
                    Success = true,
                    Message = "Payment already verified",
                    Data = new { PaymentId = payment.Id, OrderId = payment.Order?.OrderTag }
                };
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _settings.Key);

            var response = await client.GetAsync(
                $"https://api.paystack.co/transaction/verify/{transactionRef}");

            var paystackResponse =
                await response.Content.ReadFromJsonAsync<PaystackVerifyResponse>();

            if (paystackResponse == null || !paystackResponse.Status)
                return new GenericResponse
                {
                    Success = false,
                    Message = "Failed to verify payment"
                };

            if (paystackResponse.Data.Status == "success")
            {
                payment.Status = "Paid";
                payment.PaidAt = DateTime.UtcNow;

                if (payment.Order != null)
                    payment.Order.Status = "Paid";

                await _context.SaveChangesAsync();

                if (payment.Order != null)
                    await SendOrderPlacedEmail(payment.Order);

                return new GenericResponse
                {
                    Success = true,
                    Message = "Payment verified successfully",
                    Data = new { PaymentId = payment.Id, OrderId = payment.Order?.OrderTag }
                };
            }

            return new GenericResponse
            {
                Success = false,
                Message = "Payment not completed"
            };
        }
        private async Task SendOrderPlacedEmail(Order order)
        {
            if (order == null) return;

            var mailDto = new MailDto
            {
                RecipientEmail = "ikechukwungene21@gmail.com",
                Subject = $"New Order Placed: {order.OrderTag}",
                HtmlBody = $@"
            <p>Hello,</p>
            <p>A new order has been placed with Order Tag: <strong>{order.OrderTag}</strong>.</p>
            <p>Customer: {order.FirstName} {order.LastName}</p>
            <p>Email: {order.Email}</p>
            <p>Phone: {order.PhoneNumber}</p>
            <p>Total Amount: {order.TotalAmount:C}</p>
            <p>Please review the order in the admin panel.</p>
        "
            };

            await _mailService.SendEmail(mailDto);
        }
    }
}
