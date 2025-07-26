using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Application.Dtos
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string NewStatus { get; set; } = string.Empty;
    }
}
