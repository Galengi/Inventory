using System;
using System.Collections.Generic;

namespace Inventory.Models.DB
{
    public partial class User
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Mail { get; set; } = null!;
    }
}
