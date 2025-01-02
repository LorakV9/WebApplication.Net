using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Uzytkownik.ToListAsync();
            return Ok(users); // Zwraca listę wszystkich użytkowników
        }

        // POST: api/users
        [HttpPost]
        public async Task<ActionResult<User>> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Dane użytkownika są niekompletne.");
            }

            // Sprawdzanie, czy użytkownik o podanym emailu już istnieje
            var existingUser = await _context.Uzytkownik.FirstOrDefaultAsync(u => u.email == user.email);
            if (existingUser != null)
            {
                return BadRequest("Użytkownik o tym emailu już istnieje.");
            }

            // Logowanie danych użytkownika
            Console.WriteLine($"Dodawanie użytkownika: {user.imie} {user.nazwisko}, {user.email}");

            _context.Uzytkownik.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.id }, user); // Zwraca dodanego użytkownika
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

            return Ok(user); // Zwraca użytkownika o podanym id
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

            return NoContent(); // Zwraca 204 No Content po usunięciu użytkownika
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
        {
            // Sprawdzenie, czy użytkownik istnieje w bazie danych
            var user = await _context.Uzytkownik.FirstOrDefaultAsync(u => u.email == loginModel.Email);

            if (user == null)
            {
                return Unauthorized("Nie znaleziono użytkownika.");
            }

            // Sprawdzenie, czy hasło jest poprawne (porównanie jawnego hasła)
            if (user.haslo != loginModel.Password)
            {
                return Unauthorized("Niepoprawne hasło.");
            }

            return Ok(new { message = "Zalogowano pomyślnie!" }); // Użytkownik jest zalogowany
        }
    }

    // Model do logowania
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
