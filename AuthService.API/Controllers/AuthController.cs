using Microsoft.AspNetCore.Mvc;
using AuthService.API.Data;
using AuthService.API.Models;
using AuthService.API.DTOs;
using AuthService.API.Services;
using Microsoft.AspNetCore.Authorization;

namespace AuthService.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext context, JwtService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = dto.Password,
                Role = string.IsNullOrWhiteSpace(dto.Role) ? "Client" : dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok("User created");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Users
                .FirstOrDefault(x => x.Email == dto.Email && x.PasswordHash == dto.Password);

            if (user == null)
                return Unauthorized();

            var token = _jwt.GenerateToken(user);

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok("You are authorized");
        }


    }
}