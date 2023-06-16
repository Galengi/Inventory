using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class AuthRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
