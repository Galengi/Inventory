using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class Company
    {
        public Company()
        {
            ProductCompanies = new HashSet<ProductCompany>();
            Products = new HashSet<Product>();
            ShoppingLists = new HashSet<ShoppingList>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int Priority { get; set; }

        public virtual ICollection<ProductCompany> ProductCompanies { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<ShoppingList> ShoppingLists { get; set; }
    }
}
