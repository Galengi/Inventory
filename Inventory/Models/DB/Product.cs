using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class Product
    {
        public Product()
        {
            ProductCompanies = new HashSet<ProductCompany>();
            ProductTags = new HashSet<ProductTag>();
            ShoppingLists = new HashSet<ShoppingList>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int CurrentAmount { get; set; }
        public int MinAmount { get; set; }
        public byte[]? Image { get; set; }
        public decimal Price { get; set; }
        public long DefaultCompany { get; set; }
        public int Required { get; set; }
        public DateTime? Expiration { get; set; }
        public decimal ProductAmount { get; set; }
        public decimal? UnitPrice { get; set; }
        public long TypeAmount { get; set; }
        public int UnitAmount { get; set; }
        public decimal? TypePrice { get; set; }

        public virtual Company DefaultCompanyNavigation { get; set; } = null!;
        public virtual TypeUnit TypeAmountNavigation { get; set; } = null!;
        public virtual ICollection<ProductCompany> ProductCompanies { get; set; }
        public virtual ICollection<ProductTag> ProductTags { get; set; }
        public virtual ICollection<ShoppingList> ShoppingLists { get; set; }
    }
}
