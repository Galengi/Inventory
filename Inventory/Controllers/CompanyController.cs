using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventory.Models.DB;
using Inventory.Services;
using Inventory.Models.Request;
using Inventory.Models.Response;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private ICompanyService _company;

        public CompanyController(ICompanyService company)
        {
            this._company = company;
        }

        // GET: api/Company
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            Response oResponse = new Response();

            try
            {
                var lstCompanies = await _company.Get();

                if (lstCompanies == null)
                {
                    return NotFound();
                }

                oResponse.Data = lstCompanies;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // GET: api/Company/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Company>> GetCompany(long id)
        {
            Response oResponse = new Response();

            try
            {
                var company = await _company.Get(id);

                if (company == null)
                {
                    return NotFound();
                }

                oResponse.Data = company;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }

        // PUT: api/Company/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(long id, CompanyRequest company)
        {
            Response oResponse = new Response();

            try
            {
                if (id != company.Id)
                {
                    return BadRequest();
                }
                var result = await _company.Edit(id, company);
                if (result.Equals(1))
                {
                    return NotFound();
                }

                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // POST: api/Company
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Company>> PostCompany(CompanyRequest company)
        {
            Response oResponse = new Response();

            try
            {
                var result = await _company.Add(company);
                if (result.Equals(1))
                {
                    return Problem("Entity set 'InventoryContext.Companies'  is null.");
                }
                else
                {
                    //return CreatedAtAction("GetCompany", new { id = company.Id }, company);
                    oResponse.Success = 1;
                }
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);
        }

        // DELETE: api/Company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(long id)
        {
            Response oResponse = new Response();
            try
            {
                if (id == 1)
                {
                    oResponse.Message = "No se puede eliminar la empresa por defecto, debe existir almenos una";
                    return BadRequest(oResponse);
                }
                else if (id == 2)
                {
                    oResponse.Message = "No existe el campo de Empresas, error en la base de datos";
                    return BadRequest(oResponse);
                }
                else if (id == 3)
                {
                    oResponse.Message = "No existe la empresa que se desea eliminar";
                    return BadRequest(oResponse);
                }
                var result = await _company.Delete(id);

                if (result == null)
                {
                    return NotFound();
                }

                oResponse.Success = 1;
            }
            catch (Exception ex)
            {
                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }
    }
}
