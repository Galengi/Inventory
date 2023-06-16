using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class ShoppingList
    {
        public long Id { get; set; }
        public long IdProduct { get; set; }
        public int Amount { get; set; }
        public long IdCompany { get; set; }

        public virtual Company IdCompanyNavigation { get; set; } = null!;
        public virtual Product IdProductNavigation { get; set; } = null!;
    }
}
