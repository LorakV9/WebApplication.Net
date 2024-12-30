using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        [Column("kod")]
        public string Code { get; set; }
        [Column("znizka")]
        public int Discount { get; set; }
    }

}
