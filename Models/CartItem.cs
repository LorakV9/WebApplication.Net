using System.ComponentModel.DataAnnotations.Schema;

public class CartItem
{
    public int CartItemId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Amount { get; set; }

    [Column("user_id")]  // Mapowanie właściwości UserId na kolumnę user_id w bazie danych
    public int UserId { get; set; }
}