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
        public async Task<List<Category>> GetCategoriesWithProductsAsync()
        {
            var categoriesWithProducts = new List<Category>();

            var connection = this.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandText = "CALL GetCategoriesWithProducts();"; // Wywołanie procedury składowanej
            command.CommandType = System.Data.CommandType.Text;

            var reader = await command.ExecuteReaderAsync();

            // Parsowanie wyników do listy kategorii i produktów
            while (await reader.ReadAsync())
            {
                var categoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));
                var categoryName = reader.GetString(reader.GetOrdinal("CategoryName"));
                var productId = reader.IsDBNull(reader.GetOrdinal("ProductId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("ProductId"));
                var productName = reader.IsDBNull(reader.GetOrdinal("ProductName")) ? null : reader.GetString(reader.GetOrdinal("ProductName"));
                var price = reader.IsDBNull(reader.GetOrdinal("Price")) ? (double?)null : reader.GetDouble(reader.GetOrdinal("Price"));

                // Znajdź kategorię lub utwórz nową, jeśli jeszcze jej nie ma
                var category = categoriesWithProducts.FirstOrDefault(c => c.CategoryId == categoryId);
                if (category == null)
                {
                    category = new Category
                    {
                        CategoryId = categoryId,
                        Name = categoryName,
                        Products = new List<Product>()
                    };
                    categoriesWithProducts.Add(category);
                }

                // Dodaj produkt do kategorii, jeśli istnieje
                if (productId.HasValue)
                {
                    category.Products.Add(new Product
                    {
                        productid = productId.Value,
                        name = productName,
                        price = price ?? 0,
                        categoryid = categoryId
                    });
                }
            }

            return categoriesWithProducts;
        }
        public DbSet<User> Uzytkownik { get; set; } // Reprezentuje tabelę Users

        public DbSet<CartItem> Koszyk { get; set; }

        public DbSet<Order> zamowienie { get; set; }

        public DbSet<Promotion> promocje { get; set; }


    }
}

