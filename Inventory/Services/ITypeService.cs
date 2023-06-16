using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface ITypeService
    {
        public Task<ActionResult<int>> Add(TypeRequest model);
        public Task<ActionResult<TypeUnit>> Get(long Id);
        public Task<ActionResult<IEnumerable<TypeUnit>>> Get();
        public Task<ActionResult<int>> Edit(long Id, TypeRequest oModel);
        public Task<ActionResult<int>> Delete(long Id);
    }
}
