using Inventory.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class TagRequest
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int Priority { get; set; }
        public List<Product> Products { get; set; }
        public List<ProductRequest>? ProductModels { get; set; }

        public TagRequest()
        {
            this.Name = "";
            this.Products = new List<Product>();
            this.Priority = 0;
        }
    }
}
