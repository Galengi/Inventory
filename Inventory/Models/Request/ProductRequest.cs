using Inventory.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class ProductRequest
    {
        public long? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int CurrentAmount { get; set; }
        [Required]
        public int MinAmount { get; set; }
        public int UnitAmount { get; set; }
        [Required]
        public string Image { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public long DefaultCompany { get; set; }
        [Required]
        public int Required { get; set; }
        //[Required]
        //public DateTime Expiration { get; set; }
        [Required]
        public decimal ProductAmount { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        [Required]
        public long TypeAmount { get; set; }
        [Display(Name = "Lista de empresas")]
        public List<ProductCompany> ProductCompanies { get; set; }
        [Display(Name = "Lista de tags")]
        public List<ProductTag> ProductTags { get; set; }
        public ProductRequest()
        {
            this.Name = "";
            this.ProductCompanies = new List<ProductCompany>();
            this.ProductTags = new List<ProductTag>();
        }
    }
}

public class ProductCompany
{
    public long Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public ProductCompany()
    {
        this.Name = "";
        this.Id = 1;
    }
}
public class ProductTag
{
    public long Id { get; set; }
    public string Name { get; set; }
    public ProductTag()
    {
        this.Name = "";
    }
}
