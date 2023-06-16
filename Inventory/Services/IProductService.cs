using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface IProductService
    {
        public Task<ActionResult<int>> Add(ProductRequest model);
        public Task<ActionResult<ProductRequest>> Get(long Id);
        public Task<ActionResult<IEnumerable<Product>>> Get();
        public Task<ActionResult<int>> Edit(long Id, ProductRequest oModel);
        public Task<ActionResult<int>> Delete(long Id);
        public void Reload(long Id);
    }
}
