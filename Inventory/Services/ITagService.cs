using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface ITagService
    {
        public Task<ActionResult<int>> Add(TagRequest model);
        public Task<ActionResult<TagRequest>> Get(long Id);
        public Task<ActionResult<IEnumerable<Tag>>> Get();
        public Task<ActionResult<int>> Edit(long Id, TagRequest oModel);
        public Task<ActionResult<int>> Delete(long Id);
    }
}
