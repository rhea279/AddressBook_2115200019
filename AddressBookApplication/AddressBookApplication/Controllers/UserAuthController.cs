using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Service;
using ModelLayer.DTO;
using BusinessLayer.Interface;
using RepositoryLayer.Context;
using Middleware.Hashing;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Swashbuckle.AspNetCore.Annotations;

namespace AddressBookApplication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _appDbContext;
        private readonly RabbitMQProducer _rabbitMQProducer;

        public UserAuthController(AuthService authService, AppDbContext appDbContext,IEmailService emailService, RabbitMQProducer rabbitMQProducer)
        {
            _authService = authService;
            _emailService = emailService;
            _appDbContext = appDbContext;
            _rabbitMQProducer = rabbitMQProducer;
        }

        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register User")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDto)
        {
            string result = await _authService.Register(userDto);
            if (result == "User already exists") return Conflict(result);

            // Publish an event to RabbitMQ
            _rabbitMQProducer.PublishMessage(new
            {
                Event = "UserRegistered",
                Email = userDto.Email,
                Timestamp = DateTime.UtcNow
            });

            return Ok(result);
        }


        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login User")]
        public async Task<IActionResult> Login([FromBody] UserDTO userDto)
        {
            object? token = await _authService.Login(userDto);
            if (token == null) return Unauthorized("Invalid credentials");
            return Ok(new { Token = token });
        }

        [HttpPost("forgot-password")]
        [SwaggerOperation(Summary = "Forget Password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDTO request)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return BadRequest("User Not Found");

            //Generate Reset Token
            user.ResetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await _appDbContext.SaveChangesAsync();

            // Send event to RabbitMQ
            _rabbitMQProducer.PublishMessage(new
            {
                Event = "PasswordResetRequested",
                Email = user.Email,
                Timestamp = DateTime.UtcNow
            });

            return Ok("Reset password email sent.");
        }


        [HttpPost("reset-password")]
        [SwaggerOperation(Summary = "Reset Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO request)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.ResetToken == request.Token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return BadRequest("Invalid or expired token.");

            // Hash New Password using SHA-256
            user.PasswordHash = PasswordHasher.HashPassword(request.NewPassword);
            user.ResetToken = null;  // Clear reset token
            user.ResetTokenExpiry = null;
            await _appDbContext.SaveChangesAsync();

            return Ok("Password has been reset successfully.");
        }

    }
}
