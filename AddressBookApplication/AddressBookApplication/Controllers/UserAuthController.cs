using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Service;
using ModelLayer.DTO;

namespace AddressBookApplication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public UserAuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            string result = await _authService.Register(userDto);
            if (result == "User already exists") return Conflict(result);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            object? token = await _authService.Login(userDto);
            if (token == null) return Unauthorized("Invalid credentials");
            return Ok(new { Token = token });
        }
    }
}
