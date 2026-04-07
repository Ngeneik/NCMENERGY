using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Services.OrderService;
using System;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("get-all-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var result = await _orderService.GetAllOrders();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("get-order-by-Id/{orderId}")]
        public async Task<IActionResult> GetOrderById(Guid orderId)
        {
            var result = await _orderService.GetOrderById(orderId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}