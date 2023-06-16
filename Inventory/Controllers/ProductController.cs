using Inventory.Models.DB;
using Inventory.Models.Request;
using Inventory.Models.Response;
using Inventory.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private IProductService _product; //Creamos la variable sin inicializar, si en vez de poner la clase
        //ponemos la interfaz que utilizan esta clase, no nos da ningun problema,
        //con lo que seria recomendable usar siempre interafaz. No obstante, así como en el Program.cs hemos añadido
        //la inyeccion de dependencia con la interfaz y la clase, debemos añadir otra linea con la misma interfaz pero 
        // cambiando la clase que usa dicha interfaz

        public ProductController(IProductService product)  //Gracias a la inyección de dependencias en Program,
                                                     //todos los constructores del controlador tienen acceso a estos parametros
        {
            this._product = product;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            Response oResponse = new Response();

            try
            {
                var lstProducts = await _product.Get();

                if (lstProducts == null)
                {
                    return NotFound();
                }

                oResponse.Data = lstProducts;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductRequest>> GetProduct(long id)
        {
            Response oResponse = new Response();

            try
            {
                var product = await _product.Get(id);

                if (product == null)
                {
                    return NotFound();
                }

                oResponse.Data = product;
                oResponse.Success = 1;
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);


        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, ProductRequest product)
        {
            Response oResponse = new Response();

            try
            {
                /*
                if (id != product.id)
                {
                    return BadRequest();
                }
                */
                var result = await _product.Edit(id, product);
                if (result.Value.Equals(1))
                {
                    return NotFound();
                }
                else if (result.Value.Equals(2))
                {
                    oResponse.Message = "Product amount can't be 0";
                }
                else
                {
                    oResponse.Success = 1;
                }

            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductRequest product)
        {
            Response oResponse = new Response();

            try
            {
                var result = await _product.Add(product);
                if (result.Value.Equals(1))
                {
                    return Problem("Entity set 'InventoryContext.Products'  is null.");
                }
                else if (result.Value.Equals(2))
                {
                    oResponse.Message = "Product amount can't be 0";
                }
                else
                {
                    oResponse.Success = 1;
                }
            }
            catch (Exception ex)
            {

                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _product.Delete(id);

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
