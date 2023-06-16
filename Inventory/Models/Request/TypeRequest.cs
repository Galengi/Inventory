using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class TypeRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
