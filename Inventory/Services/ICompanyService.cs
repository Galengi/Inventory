using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Services
{
    public interface ICompanyService
    {
        public Task<ActionResult<int>> Add(CompanyRequest model);
        public Task<ActionResult<CompanyRequest>> Get(long Id);
        public Task<ActionResult<IEnumerable<Company>>> Get();
        public Task<ActionResult<int>> Edit(long Id, CompanyRequest oModel);
        public Task<ActionResult<int>> Delete(long Id);
    }
}
