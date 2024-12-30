using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; } // Reprezentuje tabelę Categories

        public DbSet<User> Uzytkownik { get; set; } // Reprezentuje tabelę Users

        public DbSet<CartItem> Koszyk { get; set; }

        public DbSet<Order> zamowienie { get; set; }

        public DbSet<Promotion> promocje { get; set; }


    }
}

