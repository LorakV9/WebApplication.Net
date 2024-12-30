using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var cartItems = await _context.Koszyk
                .Where(c => c.UserId == userId)  // Jeśli w modelu masz UserId
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return NotFound("Koszyk jest pusty.");
            }

            return Ok(cartItems);
        }

        // POST: api/cart
        [HttpPost]
        public async Task<ActionResult<CartItem>> AddToCart([FromBody] CartItem cartItem)
        {
            if (cartItem == null)
            {
                return BadRequest("Błąd danych.");
            }

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
