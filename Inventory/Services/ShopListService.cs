using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class ShopListService : IShopListService
    {
        delegate bool del2(ProductCompany d);
        private readonly InventoryContext _context;
        private IProductService _product;

        public ShopListService(InventoryContext context, IProductService product)
        {
            this._product = product;
            this._context = context;
        }

        public async Task<ActionResult<int>> Add(long Id, ShoppingListRequest oModel)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {

                        if (_context.Products == null)
                        {
                            throw new Exception("No existe el campo de productos. Error en la base de datos.");
                        }

                        Product? oProduct = await _context.Products.FindAsync(Id);

                        if (oProduct == null)
                        {
                            throw new Exception("No existe el producto");
                        }


                        oProduct.CurrentAmount += oModel.Amount;
                        if (oProduct.CurrentAmount < oProduct.MinAmount)
                        {
                            oModel.Amount = oProduct.MinAmount - oProduct.CurrentAmount;

                            Boolean shopExists = true;
                            ShoppingList oShop = _context.ShoppingLists.Where(d => d.IdProduct == Id).FirstOrDefault()!;

                            if (oShop == null)
                            {
                                shopExists = false;
                                oShop = new ShoppingList();
                            }


                            oShop.Amount = oModel.Amount;
                            oShop.IdCompany = oModel.IdCompany;
                            oShop.IdProduct = oModel.IdProduct;

                            if (shopExists)
                            {
                                _context.Entry(oShop).State = EntityState.Modified;
                            }
                            else
                            {
                                _context.ShoppingLists.Add(oShop);
                            }

                        }
                        else
                        {
                            ShoppingList oShop = _context.ShoppingLists.Where(d => d.IdProduct == Id).FirstOrDefault()!;

                            if (oShop != null)
                            {
                                _context.ShoppingLists.Remove(oShop);
                            }

                        }



                        _context.Entry(oProduct).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        transaction.Commit();
                        return 0;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw new Exception("Ocurrió un error en la modificacion de cantidad");
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopListExists(Id))
                {
                    return 1;
                }
                else
                {
                    throw;
                }
            }

        }

        public async Task<ActionResult<int>> Edit(long Id, ShoppingListRequest oModel)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Boolean shopExists = true;
                        ShoppingList oShop = _context.ShoppingLists.Where(d => d.IdProduct == Id).FirstOrDefault()!;

                        if (oShop == null)
                        {
                            shopExists = false;
                            oShop = new ShoppingList();
                        }


                        oShop.Amount = oModel.Amount;
                        oShop.IdCompany = oModel.IdCompany;
                        oShop.IdProduct = oModel.IdProduct;

                        if (shopExists)
                        {
                            _context.Entry(oShop).State = EntityState.Modified;
                        }
                        else
                        {
                            _context.ShoppingLists.Add(oShop);
                        }


                        await _context.SaveChangesAsync();

                        transaction.Commit();
                        return 0;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw new Exception("Ocurrió un error en la modificacion");
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShopListExists(Id))
                {
                    return 1;
                }
                else
                {
                    throw;
                }
            }

        }

        public async Task<ActionResult<int>> Delete(long Id)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    if (_context.ShoppingLists == null || _context.Products == null)
                    {
                        throw new Exception("No existe el campo de productos o la lista de la compra. Error en la base de datos.");
                    }


                    Product? oProduct = await _context.Products.FindAsync(Id);
                    ShoppingList oShopList = _context.ShoppingLists.Where(d => d.IdProduct == Id).FirstOrDefault()!;

                    if (oProduct == null || oShopList == null)
                    {
                        throw new Exception("No existe el producto o no se encuentra en la lista de la compra.");
                    }

                    oProduct.CurrentAmount += oShopList.Amount;

                    //MODIFIQUEM LA CANTITAT DEL PRODUCTE
                    _context.Entry(oProduct).State = EntityState.Modified;


                    await _context.SaveChangesAsync();


                    //ELIMINEM EL OBJECTE SHOP
                    _context.Entry(oShopList).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.ShoppingLists.Remove(oShopList);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return 0;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Ocurrió un error en la eliminación");
                }
            }
        }
        public int DeleteShop(long Id, ShoppingList oModel)
        {
            try
            {
                if (_context.ShoppingLists == null || _context.Products == null)
                {
                    return 1;
                }

                Product? oProduct = _context.Products.Find(Id);

                if (oProduct == null)
                {
                    return 1;
                }
                oProduct.CurrentAmount += oModel.Amount;

                //MODIFIQUEM LA CANTITAT DEL PRODUCTE
                _context.Entry(oProduct).State = EntityState.Modified;
                _context.SaveChanges();


                var shop = _context.ShoppingLists.Where(d => d.IdProduct == Id).FirstOrDefault();

                if (shop == null)
                {
                    return 1;
                }


                _context.SaveChanges();


                //ELIMINEM EL OBJECTE SHOP
                _context.Entry(shop).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.ShoppingLists.Remove(shop);
                _context.SaveChanges();
                return 0;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la eliminación");
            }
        }

        public async Task<ActionResult<int>> Delete()
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    if (_context.ShoppingLists == null || _context.Products == null)
                    {
                        throw new Exception("No existe el campo de productos o la lista de la compra. Error en la base de datos.");
                    }

                    List<ShoppingList> lstShop = await _context.ShoppingLists.ToListAsync();

                    if (lstShop == null)
                    {
                        throw new Exception("No existen productos en la lista de la compra.");
                    }



                    foreach (ShoppingList shopLst in lstShop)
                    {
                        var x = DeleteShop(shopLst.IdProduct, shopLst);
                    }


                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return 0;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Ocurrió un error en la eliminación");
                }
            }
        }

        public async Task<ActionResult<IEnumerable<ShopListProduct>>> Get()
        {
            try
            {
                if (_context.Companies == null)
                {
                    throw new Exception("No existe el campo de empresas. Error en la base de datos.");
                }

                List<ProductRequest> lstProductRequest = new List<ProductRequest>();
                List<ShopListProduct> lstShopProduct = new List<ShopListProduct>();


                var lstProductShop = await _context.ShoppingLists.ToListAsync();

                foreach (var prodShop in lstProductShop)
                {
                    ProductRequest prodReq = _product.Get(prodShop.IdProduct).Result.Value!;
                    ShopListProduct shoplstProd = new ShopListProduct(prodShop);
                    if (prodReq != null && !lstProductRequest.Contains(prodReq))
                    {
                        shoplstProd.ProductRequest = prodReq;
                        lstProductRequest.Add(prodReq);
                        lstShopProduct.Add(shoplstProd);
                    }
                }


                return lstShopProduct;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención de los productos de la lista de compra");
            }
        }
        private bool ShopListExists(long id)
        {
            return (_context.ShoppingLists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
