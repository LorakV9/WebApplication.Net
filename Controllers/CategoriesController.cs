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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            // Użycie Include do załadowania produktów razem z kategoriami
            var categories = await _context.Categories
                                           .Include(c => c.Products)  // Załaduj powiązane produkty
                                           .ToListAsync();

            return Ok(categories); // Zwraca listę kategorii z produktami
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<Category>> AddCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest();
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category); // Zwraca dodaną kategorię
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            // Pobierz kategorię wraz z jej produktami na podstawie id
            var category = await _context.Categories
                                         .Include(c => c.Products)  // Załaduj powiązane produkty
                                         .FirstOrDefaultAsync(c => c.CategoryId == id); // Filtruj po id

            if (category == null)
            {
                return NotFound(); // Jeśli kategoria nie istnieje, zwróć 404
            }

            return Ok(category); // Zwróć znalezioną kategorię wraz z produktami
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // Znajdź kategorię w bazie danych
            var category = await _context.Categories
                                          .Include(c => c.Products)  // Załaduj powiązane produkty
                                          .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
            {
                return NotFound(); // Jeśli kategoria nie istnieje, zwróć 404
            }

            // Usuń powiązane produkty
            if (category.Products != null)
            {
                _context.Products.RemoveRange(category.Products);  // Usuwa wszystkie produkty przypisane do kategorii
            }

            // Usuń kategorię
            _context.Categories.Remove(category);

            // Zapisz zmiany w bazie danych
            await _context.SaveChangesAsync();

            return NoContent(); // Zwraca 204 No Content po usunięciu kategorii i produktów
        }

        [HttpGet("categories-with-products")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoriesWithProducts()
        {
            var categoriesWithProducts = await _context.GetCategoriesWithProductsAsync();
            return Ok(categoriesWithProducts);
        }


    }
}
