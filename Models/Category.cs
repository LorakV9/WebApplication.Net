namespace WebApplication1.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // Id kategorii
        public string Name { get; set; } // Nazwa kategorii
        public List<Product> Products { get; set; } // Lista produktów przypisanych do kategorii
    }
}

