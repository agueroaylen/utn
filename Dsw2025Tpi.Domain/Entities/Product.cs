using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities
{
    public class Product : EntityBase
    {
        [Required]
        public string Sku { get; set; } = null!;

        public string InternalCode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal CurrentUnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
