using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // Endpoint do tworzenia nowego zamówienia
        [HttpPost("create/{userId}")]
        public async Task<IActionResult> CreateOrder(int userId)
        {
            // Pobranie produktów z koszyka użytkownika
            var cartItems = await _context.Koszyk
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest("Koszyk jest pusty.");
            }

            // Tworzenie opisu zamówienia i obliczenie łącznej ceny
            string description = string.Join(", ", cartItems.Select(c => $"{c.Name} (x{c.Amount}, {c.Price} zł)"));
            double totalPrice = (double)cartItems.Sum(c => c.Price * c.Amount);

            // Utworzenie nowego zamówienia
            var newOrder = new Order
            {
                UserId = userId,
                Description = description,
                TotalPrice = totalPrice,
                Date = DateTime.Now,
                Approved = false
            };

            // Dodanie zamówienia do bazy danych
            _context.zamowienie.Add(newOrder);

            // Usunięcie produktów z koszyka po złożeniu zamówienia
            _context.Koszyk.RemoveRange(cartItems);

            // Zapisanie zmian w bazie danych
            await _context.SaveChangesAsync();

            return Ok(newOrder);
        }

        // Endpoint do wyświetlania zamówień użytkownika
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetOrders(int userId)
        {
            var orders = await _context.zamowienie
                .Where(o => o.UserId == userId)
                .ToListAsync();

            if (!orders.Any())
            {
                return NotFound("Brak zamówień dla podanego użytkownika.");
            }

            return Ok(orders);
        }


        [HttpPut("approve/{orderId}")]
        public async Task<IActionResult> ApproveOrder(int orderId)
        {
            // Pobranie zamówienia na podstawie ID
            var order = await _context.zamowienie.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound("Zamówienie o podanym ID nie istnieje.");
            }

            // Sprawdzenie, czy zamówienie jest już zatwierdzone
            if (order.Approved)
            {
                return Ok($"Zamówienie o ID {orderId} jest już zatwierdzone.");
            }

            // Zmiana statusu na zatwierdzone
            order.Approved = true;

            // Zapisanie zmian w bazie danych
            await _context.SaveChangesAsync();

            return Ok($"Zamówienie o ID {orderId} zostało zatwierdzone.");
        }


        [HttpPost("ApplyPromotion")]
        public async Task<IActionResult> ApplyPromotion(int orderId, string promoCode)
        {
            // Znajdź kod promocyjny w bazie danych
            var promotion = await _context.promocje.FirstOrDefaultAsync(p => p.Code == promoCode);

            if (promotion == null)
            {
                return BadRequest("Kod promocyjny nie istnieje.");
            }

            // Znajdź zamówienie na podstawie przekazanego orderId
            var order = await _context.zamowienie.FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return BadRequest("Zamówienie nie istnieje.");
            }

            // Sprawdź, czy kod już został zastosowany
            if (!string.IsNullOrEmpty(order.CodeApplied))
            {
                return BadRequest("Kod już został użyty.");
            }

            // Oblicz nową cenę po zastosowaniu zniżki
            double originalPrice = order.TotalPrice;  // Oryginalna cena zamówienia
            double discountAmount = originalPrice * (promotion.Discount / 100.0);  // Oblicz zniżkę
            double newPrice = originalPrice - discountAmount;  // Nowa cena po zniżce

            // Zaktualizuj cenę zamówienia
            order.TotalPrice = newPrice;
            order.CodeApplied = promoCode;  // Zapisz kod promocyjny jako użyty
             // Opcjonalnie, zapisz kwotę zniżki (jeśli dodasz tę kolumnę)

            // Zapisz zmiany w bazie danych
            _context.zamowienie.Update(order);
            await _context.SaveChangesAsync();

            // Zwróć odpowiedź z nową ceną
            return Ok($"Kod promocyjny '{promoCode}' został zastosowany. Nowa cena zamówienia to {newPrice:F2} zł.");
        }

    }
}
