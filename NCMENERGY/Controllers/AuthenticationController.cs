using Microsoft.AspNetCore.Mvc;
using NCMENERGY.Dtos;
using NCMENERGY.Services.AuthService;
using System.Threading.Tasks;

namespace NCMENERGY.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto request)
        {
            var result = await _authService.SignUp(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login request)
        {
            var result = await _authService.Login(request);
            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}