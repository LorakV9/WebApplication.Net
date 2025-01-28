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

        // GET: api/users/with-cart
        [HttpGet("with-cart")]
        public async Task<ActionResult<List<User>>> GetUsersWithCartItems()
        {
            try
            {
                // Wywołanie metody, która pobiera użytkowników z ich koszykami
                var users = await GetUsersWithCartItemsAsync();

                if (users == null || users.Count == 0)
                {
                    return NotFound("No users found with cart items.");
                }

                return Ok(users); // Zwraca dane użytkowników z ich elementami koszyka
            }
            catch (System.Exception ex)
            {
                // Obsługa błędów
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Zdefiniuj metodę GetUsersWithCartItemsAsync w tym kontrolerze
        private async Task<List<User>> GetUsersWithCartItemsAsync()
        {
            var usersWithCartItems = new List<User>();

            var connection = _context.Database.GetDbConnection(); // Użyj kontekstu bazy danych
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandText = "CALL GetUserWithCartItems();"; // Wywołanie procedury składowanej
            command.CommandType = System.Data.CommandType.Text;

            var reader = await command.ExecuteReaderAsync();

            // Parsowanie wyników do listy użytkowników i elementów koszyka
            while (await reader.ReadAsync())
            {
                var userId = reader.GetInt32(reader.GetOrdinal("UserId"));
                var firstName = reader.GetString(reader.GetOrdinal("FirstName"));
                var lastName = reader.GetString(reader.GetOrdinal("LastName"));
                var email = reader.GetString(reader.GetOrdinal("Email"));
                var cartItemId = reader.IsDBNull(reader.GetOrdinal("CartItemId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("CartItemId"));
                var productId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ProductId"));
                var productName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName"));
                var price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Price"));
                var amount = reader.IsDBNull(reader.GetOrdinal("Amount")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Amount"));

                // Znajdź użytkownika lub utwórz nowego, jeśli jeszcze go nie ma
                var user = usersWithCartItems.FirstOrDefault(u => u.id == userId);
                if (user == null)
                {
                    user = new User
                    {
                        id = userId,
                        imie = firstName,
                        nazwisko = lastName,
                        email = email,
                        CartItems = new List<CartItem>() // Dodano listę CartItems
                    };
                    usersWithCartItems.Add(user);
                }

                // Dodaj element koszyka do użytkownika, jeśli istnieje
                if (cartItemId.HasValue)
                {
                    user.CartItems.Add(new CartItem
                    {
                        CartItemId = cartItemId.Value,
                        ProductId = productId ?? 0,
                        Name = productName,
                        Price = price ?? 0,
                        Amount = amount ?? 0,
                        UserId = userId
                    });
                }
            }

            return usersWithCartItems;
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
        public async Task<ActionResult<User>> CreateUserWithCart([FromBody] User userInput)
        {
            if (userInput == null || userInput.CartItems == null || userInput.CartItems.Count == 0)
            {
                return BadRequest("Invalid data provided. User and CartItems are required.");
            }

            // Tworzymy nowego użytkownika
            var newUser = new User
            {
                imie = userInput.imie,
                nazwisko = userInput.nazwisko,
                email = userInput.email,
                haslo = userInput.haslo
            };

            // Dodajemy użytkownika do bazy danych
            _context.Uzytkownik.Add(newUser);
            await _context.SaveChangesAsync();

            // Dodajemy produkty do koszyka nowego użytkownika
            foreach (var cartItemInput in userInput.CartItems)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.productid == cartItemInput.ProductId);

                if (product == null)
                {
                    return NotFound($"Product with ID {cartItemInput.ProductId} not found.");
                }

                // Tworzymy nowy CartItem
                var cartItem = new CartItem
                {
                    ProductId = cartItemInput.ProductId,
                    UserId = newUser.id,
                    Amount = cartItemInput.Amount,
                    Name = product.name,
                    Price = (decimal)product.price
                };

                // Sprawdzamy, czy produkt już istnieje w koszyku tego użytkownika
                var existingCartItem = await _context.Koszyk
                    .FirstOrDefaultAsync(c => c.ProductId == cartItem.ProductId && c.UserId == cartItem.UserId);

                if (existingCartItem != null)
                {
                    // Jeśli produkt już jest w koszyku, zwiększamy ilość
                    existingCartItem.Amount += cartItem.Amount;
                    _context.Koszyk.Update(existingCartItem);
                }
                else
                {
                    // Jeśli to nowy produkt, dodajemy go do koszyka
                    _context.Koszyk.Add(cartItem);
                }
            }

            // Zapisujemy zmiany w bazie
            await _context.SaveChangesAsync();

            // Zwracamy odpowiedź z nowo utworzonym użytkownikiem
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.id }, newUser);
        }

        // GET: api/users/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _context.Uzytkownik.Include(u => u.CartItems).FirstOrDefaultAsync(u => u.id == id);

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
