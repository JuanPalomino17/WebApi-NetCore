using Microsoft.AspNetCore.Mvc;
using ApiVehiculos.Context;
using ApiVehiculos.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace ApiVehiculos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController: Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!IsValidEmail(user.Email))
                return BadRequest("Invalid email address");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                return BadRequest("Email is already registered");

            if (!IsValidPassword(user.PasswordHash))
                return BadRequest("Invalid password");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (!IsValidEmail(user.Email))
                return BadRequest("Invalid email address");

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser == null)
                return BadRequest("User not found");

            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, existingUser.PasswordHash))
                return BadRequest("Incorrect password");

            var token = GenerateJwtToken(existingUser);

            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool IsValidPassword(string password)
        {
            return password.Length >= 10 &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsUpper) &&
                   password.Any(ch => "!@#?]".Contains(ch));
        }
    }
}
