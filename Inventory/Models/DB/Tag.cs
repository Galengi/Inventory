using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class Tag
    {
        public Tag()
        {
            ProductTags = new HashSet<ProductTag>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public int Priority { get; set; }

        public virtual ICollection<ProductTag> ProductTags { get; set; }
    }
}
