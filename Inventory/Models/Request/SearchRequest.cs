using Inventory.Models.DB;
using Inventory.Models.OwnModels;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class SearchRequest
    {
        [Required]
        public string Name { get; set; }
        [Display(Name = "Lista de productos")]
        public List<Product> Products { get; set; }
        [Display(Name = "Lista de empresas")]
        public List<Company> Companies { get; set; }
        [Display(Name = "Lista de tags")]
        public List<Tag> Tags { get; set; }
        //[Display(Name = "Lista de Items comparador")]
        //public List<BasicItem> Items { get; set; }
        public SearchRequest()
        {
            this.Products = new List<Product>();
            this.Companies = new List<Company>();
            this.Tags = new List<Tag>();
            //this.Items = new List<BasicItem>();
        }
    }
}
