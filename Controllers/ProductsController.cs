using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        // GET: api/products/category/{id}
        [HttpGet("category/{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int id)
        {
            var products = await _context.Products.Where(p => p.categoryid == id).ToListAsync();
            return Ok(products);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            // Dodaj produkt do bazy danych
            _context.Products.Add(product);

            // Zatwierdź zmiany w bazie danych
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.productid }, product);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }



        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent(); // Return 204 No Content if deletion was successful
        }
    }
}
