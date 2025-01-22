using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Uzytkownik.ToListAsync();
            return Ok(users);
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Dane użytkownika są niekompletne.");
            }

            var existingUser = await _context.Uzytkownik.FirstOrDefaultAsync(u => u.email == user.email);
            if (existingUser != null)
            {
                return BadRequest("Użytkownik o tym emailu już istnieje.");
            }

            _context.Uzytkownik.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Uzytkownik.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // DELETE: api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Uzytkownik.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Uzytkownik.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Uzytkownik.FirstOrDefaultAsync(u => u.email == loginModel.Email);

            if (user == null || user.haslo != loginModel.Password)
            {
                return Unauthorized("Niepoprawny email lub hasło.");
            }

            // Pobranie wartości z konfiguracji
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // Generowanie JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.email) }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString, Message = "Zalogowano pomyślnie!" });
        }
    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
