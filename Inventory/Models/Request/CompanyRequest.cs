using Inventory.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class CompanyRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Priority { get; set; }
        [Display(Name = "Lista de productos")]
        public List<Product> Products { get; set; }
        public List<ProductRequest>? ProductModels { get; set; }

        public CompanyRequest()
        {
            this.Products = new List<Product>();
            this.Priority = 0;
        }
    }
}
