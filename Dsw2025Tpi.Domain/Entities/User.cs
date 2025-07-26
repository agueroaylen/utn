using System.ComponentModel.DataAnnotations;

namespace Dsw2025Tpi.Domain.Entities
{
    public class User : EntityBase
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
