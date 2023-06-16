using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class ProductTag
    {
        public long Id { get; set; }
        public long IdProduct { get; set; }
        public long IdTag { get; set; }

        public virtual Product IdProductNavigation { get; set; } = null!;
        public virtual Tag IdTagNavigation { get; set; } = null!;
    }
}
