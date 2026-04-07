using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Services.DashboardService;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashBoardController : ControllerBase
    {
        private readonly IDashBoardService _dashBoardService;

        public DashBoardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet("cards")]
        public async Task<IActionResult> GetCards()
        {
            var result = await _dashBoardService.GetCards();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            var result = await _dashBoardService.GetResentOrders();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}