using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Services.SettingsService;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SettingsController : ControllerBase
    {
        private readonly ISettingsService _settingsService;

        public SettingsController(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [HttpGet("get-status")]
        public async Task<IActionResult> GetStatus()
        {
            var result = await _settingsService.GetStatus();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("add-settings")]
        public async Task<IActionResult> AddSettings()
        {
            var result = await _settingsService.AddSettings();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("status/change")]
        public async Task<IActionResult> ChangeStatus()
        {
            var result = await _settingsService.ChangeStatus();
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}