using System.ComponentModel.DataAnnotations;

namespace Tienda.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        public Product? Product { get; set; }
    }
}