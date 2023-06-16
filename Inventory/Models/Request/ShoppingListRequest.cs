using Inventory.Models.DB;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Request
{
    public class ShoppingListRequest
    {
        [Required]
        public long IdProduct { get; set; }
        [Required]
        public long IdCompany { get; set; }
        [Required]
        public int Amount { get; set; }
    }
    public class ShopListProduct
    {
        [Required]
        public long IdProduct { get; set; }
        [Required]
        public long IdCompany { get; set; }
        [Required]
        public int Amount { get; set; }
        public ProductRequest? ProductRequest { get; set; }

        
        public ShopListProduct(ShoppingList shopListRequest)
            {
            this.IdProduct = shopListRequest.IdProduct;
            this.IdCompany = shopListRequest.IdCompany;
            this.Amount = shopListRequest.Amount;
        }
    }

}
