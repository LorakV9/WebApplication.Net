using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    [Authorize]
    public class PromotionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromotionController(AppDbContext context)
        {
            _context = context;
        }

        // Wyświetlanie wszystkich promocji
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await _context.promocje.ToListAsync();
            return Ok(promotions);
        }

        // Dodawanie nowej promocji
        [HttpPost]
        public async Task<IActionResult> AddPromotion([FromBody] Promotion promotion)
        {
            if (promotion == null || string.IsNullOrEmpty(promotion.Code) || promotion.Discount <= 0)
            {
                return BadRequest("Nieprawidłowe dane promocji.");
            }

            _context.promocje.Add(promotion);
            await _context.SaveChangesAsync();

            return Ok($"Promocja o kodzie '{promotion.Code}' została dodana.");
        }

        // Usuwanie promocji na podstawie ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            var promotion = await _context.promocje.FindAsync(id);

            if (promotion == null)
            {
                return NotFound("Promocja o podanym ID nie istnieje.");
            }

            _context.promocje.Remove(promotion);
            await _context.SaveChangesAsync();

            return Ok($"Promocja o ID {id} została usunięta.");
        }
    }

}
