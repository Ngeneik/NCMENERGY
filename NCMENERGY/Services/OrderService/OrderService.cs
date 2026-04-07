using Microsoft.EntityFrameworkCore;
using NCMENERGY.Data;
using NCMENERGY.Response;

namespace NCMENERGY.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<GenericResponse> GetAllOrders()
        {
            var orders = await _context.Orders
                .Select(o => new
                {
                    o.Id,
                    o.OrderTag,
                    FullName = o.FirstName + " " + o.LastName,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt
                })
                .ToListAsync();

            return new GenericResponse
            {
                Success = true,
                Data = orders
            };
        }

        public async Task<GenericResponse> GetOrderById(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Payments) // single Payment navigation
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "Order not found"
                };
            }

            var payment = order.Payments; 

            var data = new
            {
                order.OrderTag,
                order.Status,
                CustomerInfo = new
                {
                    FullName = order.FirstName + " " + order.LastName,
                    order.Email,
                    order.PhoneNumber
                },
                DeliveryInfo = new
                {
                    order.Address,
                    order.City,
                    order.State,
                    order.DeliveryMethod
                },
                PaymentInfo = new
                {
                    payment.PaymentMethod,
                    payment.Status,
                    payment.TransactionRef,
                    payment.PaidAt
                },
                OrderItems = order.OrderItems.Select(oi => new
                {
                    oi.ProductName,
                    oi.UnitPrice,
                    oi.Quantity,
                    oi.TotalAmount
                }).ToList()
            };

            return new GenericResponse
            {
                Success = true,
                Data = data
            };
        }
    }
}
