﻿using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Service;
using ModelLayer.DTO;
using BusinessLayer.Interface;
using RepositoryLayer.Context;
using Middleware.Hashing;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AddressBookApplication.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserAuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _appDbContext;

        public UserAuthController(AuthService authService, AppDbContext appDbContext,IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
            _appDbContext = appDbContext;
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordDTO request)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return BadRequest("User Not Found");

            //Generate Reset Token
            user.ResetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await _appDbContext.SaveChangesAsync();

            // Construct Reset Link
            var resetLink = $"{Request.Scheme}://{Request.Host}/reset-password?token={user.ResetToken}";

            // Log the reset token for debugging
            Console.WriteLine($"Reset Token: {user.ResetToken}");

            // Send Email
            await _emailService.SendEmail(
                 user.Email,
    "Password Reset",
    $"<h3>Reset Your Password</h3>" +
    $"<p>Your Reset Token: <strong>{user.ResetToken}</strong></p>"
            );

            return Ok("Reset password email sent.");
        }

        [HttpPost("reset-password")]
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
