using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/cart/{userId}
        [HttpGet("cart/{userId}")]
        public async Task<IActionResult> GetCartItems(int userId)
        {
            Console.WriteLine($"Pobieranie koszyka dla użytkownika o ID: {userId}");

            var cartItems = await _context.Koszyk
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                Console.WriteLine("Koszyk jest pusty."); // Log pustego koszyka
                return NotFound("Koszyk jest pusty.");
            }

            Console.WriteLine($"Znaleziono {cartItems.Count} elementów w koszyku:");
            foreach (var item in cartItems)
            {
                Console.WriteLine($"- {item.Name}, Ilość: {item.Amount}, Cena: {item.Price}");
            }

            return Ok(cartItems);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItem cartItemInput)
        {
            if (cartItemInput == null || cartItemInput.ProductId <= 0 || cartItemInput.Amount <= 0 || cartItemInput.UserId <= 0)
            {
                return BadRequest("Invalid data provided.");
            }

            // Pobierz produkt z tabeli Product na podstawie ProductId
            var product = await _context.Products.FirstOrDefaultAsync(p => p.productid == cartItemInput.ProductId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Uzupełnij brakujące dane w CartItem
            var cartItem = new CartItem
            {
                ProductId = cartItemInput.ProductId,
                UserId = cartItemInput.UserId,
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

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCartItems), new { userId = cartItem.UserId }, cartItem); // Zwraca nowy lub zaktualizowany produkt w koszyku
        }

        // DELETE: api/cart/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.Koszyk.FindAsync(id);

            if (cartItem == null)
            {
                return NotFound("Produkt nie znaleziony w koszyku.");
            }

            _context.Koszyk.Remove(cartItem);
            await _context.SaveChangesAsync();

            return NoContent(); // Zwraca 204 No Content po usunięciu produktu z koszyka
        }
    }
}
