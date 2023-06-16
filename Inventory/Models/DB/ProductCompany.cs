using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class ProductCompany
    {
        public long Id { get; set; }
        public long IdProduct { get; set; }
        public long IdCompany { get; set; }
        public decimal Price { get; set; }

        public virtual Company IdCompanyNavigation { get; set; } = null!;
        public virtual Product IdProductNavigation { get; set; } = null!;
    }
}
