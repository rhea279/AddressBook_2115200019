using System;
using Middleware.Hashing;
using Middleware.JWT_Token;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Context;
using Microsoft.EntityFrameworkCore;
using BusinessLayer.Interface;

namespace BusinessLayer.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string> Register(UserDTO userDto)
        {
            // Check if user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null) return "User already exists";

            // Hash the password
            string hashedPassword = PasswordHasher.HashPassword(userDto.Password);

            // Create new user
            var newUser = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                PasswordHash = hashedPassword
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return "User registered successfully";
        }

        public async Task<object?> Login(UserDTO userDto)
        {
            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null) return new { Message = "Invalid Email or Password" }; // User not found

            // Verify password
            string hashedPassword = PasswordHasher.HashPassword(userDto.Password);
            if (hashedPassword != user.PasswordHash) return new { Message = "Invalid Email or Password" }; // Invalid credentials

            // Generate JWT Token
            string token = _jwtService.GenerateToken(user.Id.ToString(), user.Email);

            return new { Message = "Login Successful", Token = token };
        }
    }
}
