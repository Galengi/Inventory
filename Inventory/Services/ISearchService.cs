using Inventory.Models.DB;
using Inventory.Models.OwnModels;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface ISearchService
    {
        public Task<ActionResult<SearchRequest>> Get(string name);
        public Task<IEnumerable<BasicItem>> GetFromCarrefour(string name);
        public Task<IEnumerable<BasicItem>> GetFromConsum(string name);
        public Task<IEnumerable<BasicItem>> GetFromDia(string name);
        public Task<IEnumerable<BasicItem>> GetFromMercadona(string name);
    }
}
