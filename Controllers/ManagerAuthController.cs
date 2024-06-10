using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using terrain.Models;
using System.ComponentModel.DataAnnotations;

namespace terrain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerAuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ManagerAuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var manager = await _context.Managers.SingleOrDefaultAsync(m => m.Nom == model.Email);
            if (manager == null || !BCrypt.Net.BCrypt.Verify(model.Password, manager.Password))
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(manager.Nom);
            return Ok(new { token });
        }

        private string GenerateJwtToken(string nom)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nom),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}