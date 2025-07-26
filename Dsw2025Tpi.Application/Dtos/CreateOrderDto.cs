using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Application.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        public string BillingAddress { get; set; } = string.Empty;

        [Required]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Opcionales, los podés ignorar en el backend si querés
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? CurrentUnitPrice { get; set; }
    }
}
