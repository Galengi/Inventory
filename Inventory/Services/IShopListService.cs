using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface IShopListService
    {
        public Task<ActionResult<IEnumerable<ShopListProduct>>> Get();
        public Task<ActionResult<int>> Edit(long Id, ShoppingListRequest oModel);
        //public Task<ActionResult<int>> Edit(IEnumerable<ShoppingListRequest> lstModel);
        public Task<ActionResult<int>> Add(long Id, ShoppingListRequest oModel);
        //public Task<ActionResult<int>> Add(IEnumerable<ShoppingListRequest> lstModel);
        public Task<ActionResult<int>> Delete(long Id);
        public Task<ActionResult<int>> Delete();
        //public Task<ActionResult<int>> Delete(IEnumerable<ShoppingListRequest> lstModel);
    }
}
