using System;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    public int Id { get; set; }
    [Column("uzytkownik_id")]
    public int UserId { get; set; }
    [Column("opis")]
    public string Description { get; set; }
    [Column("cena")]
    public double TotalPrice { get; set; }
    [Column("data")]
    public DateTime Date { get; set; }

    [Column("zatwierdzone")]
    public bool Approved { get; set; }
    [Column("kod_promocyjny")]
    public string? CodeApplied { get; set; } // Nowe pole
}
