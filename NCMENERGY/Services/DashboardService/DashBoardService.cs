using Microsoft.EntityFrameworkCore;
using NCMENERGY.Data;
using NCMENERGY.Response;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NCMENERGY.Services.DashboardService
{
    public class DashBoardService : IDashBoardService
    {
        private readonly ApplicationDbContext _context;

        public DashBoardService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<GenericResponse> GetCards()
        {
            var totalOrders = _context.Orders.Count();
            var totalRevenue = _context.Orders.Sum(o => o.TotalAmount);
            var totalProducts = _context.Products.Count();
            var data = new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalProducts = totalProducts
            };
            return new GenericResponse
            {
                Success = true,
                Data = data
            };
        }

        public async Task<GenericResponse> GetResentOrders()
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

    }
}
