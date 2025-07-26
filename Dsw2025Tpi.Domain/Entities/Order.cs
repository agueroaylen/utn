using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Order : EntityBase
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string ShippingAddress { get; set; } = null!;

        [Required]
        public string BillingAddress { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
