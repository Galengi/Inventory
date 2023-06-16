using Inventory.Models.DB;
using Inventory.Models.Request;
using Inventory.Models.Response;
using Inventory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private ITypeService _typeP;

        public TypeController(ITypeService typeP)
        {
            this._typeP = typeP;
        }

        // GET: api/Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeUnit>>> GetTypes()
        {
            Response oResponse = new Response();

            try
            {
                var lstTypes = await _typeP.Get();

                if (lstTypes == null)
                {
                    return NotFound();
                }

                oResponse.Data = lstTypes;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // GET: api/Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TypeUnit>> GetType(long id)
        {
            Response oResponse = new Response();

            try
            {
                var typeP = await _typeP.Get(id);

                if (typeP == null)
                {
                    return NotFound();
                }

                oResponse.Data = typeP;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }

        // PUT: api/Type/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutType(long id, TypeRequest typeP)
        {
            Response oResponse = new Response();

            try
            {
                /*
                if (id != typeP.Id)
                {
                    return BadRequest();
                }
                */
                var result = await _typeP.Edit(id, typeP);
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

        // POST: api/Type
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TypeUnit>> PostType(TypeRequest typeP)
        {
            Response oResponse = new Response();

            try
            {
                var result = await _typeP.Add(typeP);
                if (result.Equals(1))
                {
                    return Problem("Entity set 'InventoryContext.'  is null.");
                }
                else
                {
                    //return CreatedAtAction("GetType", new { id = typeP.Id }, typeP);
                    oResponse.Success = 1;
                }
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);
        }

        // DELETE: api/Type/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteType(long id)
        {
            Response oResponse = new Response();
            try
            {
                if (id == 1)
                {
                    oResponse.Message = "No se puede eliminar el tipo por defecto, debe existir almenos uno";
                    return BadRequest(oResponse);
                }
                var result = await _typeP.Delete(id);

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
