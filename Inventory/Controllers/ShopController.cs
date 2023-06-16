using Inventory.Models.Request;
using Inventory.Models.Response;
using Inventory.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private IShopListService _shopList;

        public ShopController(IShopListService shopList)
        {
            this._shopList = shopList;
        }

        // GET: api/Product
        //DEVUELVE UNA LISTA D ELOS PRODCUTOS QUE TENEMOS EN LA LISTA DE LA COMPRA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRequest>>> GetProducts()
        {
            Response oResponse = new Response();

            try
            {
                var lstProducts = await _shopList.Get();

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



        [HttpPost()]
        public async Task<IActionResult> PostProduct(IEnumerable<ShoppingListRequest> shoppingList)
        {
            Response oResponse = new Response();
            try
            {
                //var result = await _shopList.Delete(shoppingList);
                foreach (ShoppingListRequest shopLst in shoppingList)
                {
                    var result = await _shopList.Add(shopLst.IdProduct, shopLst);
                    if (result == null)
                    {
                        return NotFound();
                    }
                }


                oResponse.Success = 1;
            }
            catch (Exception ex)
            {
                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }

        
        //POST: ENVIEM DADES QUE MODIFIQUEN CURRENT AMOUNT DE PRODUCTE EN QÜESTIÓ
        [HttpPost("{id}")]
        public async Task<IActionResult> PostProduct(long id, ShoppingListRequest shoppingRequest)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _shopList.Add(id, shoppingRequest);

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


        // DELETE: api/Product/5
        // REMOVES PRODUCT FROM SHOPPING LIST AND ACTUALIZE COUNT
        [HttpPut()]
        public async Task<IActionResult> PutProduct(IEnumerable<ShoppingListRequest> shoppingList)
        {
            Response oResponse = new Response();
            try
            {
                //var result = await _shopList.Delete(shoppingList);
                foreach (ShoppingListRequest shopLst in shoppingList)
                {
                    var result = await _shopList.Edit(shopLst.IdProduct, shopLst);
                    if (result == null)
                    {
                        return NotFound();
                    }
                }


                oResponse.Success = 1;
            }
            catch (Exception ex)
            {
                oResponse.Message = ex.Message;
            }

            return Ok(oResponse);

        }


        //PUT: ENVIEM DADES QUE MODIFIQUEN EL SHOP LIST EN QÜESTIÓ
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(long id, ShoppingListRequest shoppingRequest)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _shopList.Edit(id, shoppingRequest);

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




        // DELETE: api/Product/5
        // DELETE: ELIMINEM EL ELEMENT SHOPLIST, MODIFIQUEM EL PRODUCTE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(long id)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _shopList.Delete(id);

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


        // DELETE: api/Product/5
        // REMOVES PRODUCT FROM SHOPPING LIST AND ACTUALIZE COUNT
        [HttpDelete()]
        public async Task<IActionResult> Delete(IEnumerable<ShoppingListRequest> shoppingList)
        {
            Response oResponse = new Response();
            try
            {
                var result = await _shopList.Delete();
                /*
                foreach (ShoppingListRequest shopLst in shoppingList)
                {
                    var result = await _shopList.Delete();
                    if (result == null)
                    {
                        return NotFound();
                    }
                }
                */


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
