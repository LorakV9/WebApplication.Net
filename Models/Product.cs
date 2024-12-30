using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
   

    public class Product
    {
        [Key]
        public int productid { get; set; }
        public string name { get; set; }
        public double price { get; set; }
        public int categoryid { get; set; }
    }

}
