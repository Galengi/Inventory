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
    public class TagController : ControllerBase
    {
        private ITagService _tag;

        public TagController(ITagService tag)
        {
            this._tag = tag;
        }

        // GET: api/Tag
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            Response oResponse = new Response();

            try
            {
                var lstTags = await _tag.Get();

                if (lstTags == null)
                {
                    return NotFound();
                }

                oResponse.Data = lstTags;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // GET: api/Tag/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tag>> GetTag(long id)
        {
            Response oResponse = new Response();

            try
            {
                var tag = await _tag.Get(id);

                if (tag == null)
                {
                    return NotFound();
                }

                oResponse.Data = tag;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }

        // PUT: api/Tag/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTag(long id, TagRequest tag)
        {
            Response oResponse = new Response();

            try
            {
                /*
                if (id != tag.Id)
                {
                    return BadRequest();
                }
                */
                var result = await _tag.Edit(id, tag);
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

        // POST: api/Tag
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tag>> PostTag(TagRequest tag)
        {
            Response oResponse = new Response();

            try
            {
                var result = await _tag.Add(tag);
                if (result.Equals(1))
                {
                    return Problem("Entity set 'InventoryContext.'  is null.");
                }
                else
                {
                    //return CreatedAtAction("GetTag", new { id = tag.Id }, tag);
                    oResponse.Success = 1;
                }
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);
        }

        // DELETE: api/Tag/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(long id)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _tag.Delete(id);

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
