using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Application.Dtos
{
    public class ProductCreateDto
    {
        [Required]
        public string Sku { get; set; } = "";

        public string InternalCode { get; set; } = "";

        [Required]
        public string Name { get; set; } = "";

        public string? Description { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal CurrentUnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}
