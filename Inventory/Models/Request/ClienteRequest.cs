using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class ClienteRequest
    {
        public int Id { get; set; }
        [Required]
        [Display(Name= "Nombre del cliente")]
        public string Nombre { get; set; }
    }
}
