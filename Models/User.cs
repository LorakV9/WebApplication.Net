namespace WebApplication1.Models
{
    public class User
    {
        public int id { get; set; }    // Id użytkownika
        public string imie { get; set; }   // Imię użytkownika
        public string nazwisko { get; set; }    // Nazwisko użytkownika
        public string email { get; set; }       // Email użytkownika
        public string haslo { get; set; }    // Hasło użytkownika
        public List<CartItem> CartItems { get; set; } // Elementy koszyka przypisane do użytkownika
    }

    public class CartItemInput
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
    }
}
