using System;
using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        // Estos campos los completa el sistema, no el cliente
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal UnitPrice { get; set; }


        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }


        public Order? Order { get; set; }
    }
}
