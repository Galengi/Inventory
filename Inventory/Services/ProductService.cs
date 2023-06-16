using AutoMapper;
using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Inventory.Services
{
    public class ProductService : IProductService
    {
        delegate bool del2(ProductCompany d);
        private readonly InventoryContext _context;
        //private bool _enableWriteCount;

        public ProductService(InventoryContext context)
        {
            this._context = context;
        }

        public async Task<ActionResult<int>> Delete(long Id)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Products == null)
                    {
                        throw new Exception("No existe el campo de productos. Error en la base de datos.");
                    }
                    var oProduct = await _context.Products.FindAsync(Id);
                    if (oProduct == null)
                    {
                        throw new Exception("No existe el producto.");
                    }

                    var lstCompanies = _context.ProductCompanies.Where((d) => d.IdProduct == oProduct.Id).ToList();

                    foreach (var company in lstCompanies)
                    {
                        _context.Remove(company);
                    }
                    _context.SaveChanges();

                    var lstTags = _context.ProductTags.Where((d) => d.IdProduct == oProduct.Id).ToList();

                    foreach (var tag in lstTags)
                    {
                        _context.Remove(tag);
                    }
                    _context.SaveChanges();

                    var lstShop = _context.ShoppingLists.Where((d) => d.IdProduct == oProduct.Id).ToList();
                    foreach (var shop in lstShop)
                    {
                        _context.Remove(shop);
                    }
                    _context.SaveChanges();

                    _context.Remove(oProduct);
                    _context.SaveChanges();
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

        public async Task<ActionResult<int>> Edit(long id, ProductRequest oModel)
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

                        var oProduct = _context.Products.Find(id)!;


                        oProduct.Name = oModel.Name;
                        oProduct.CurrentAmount = oModel.CurrentAmount;
                        oProduct.MinAmount = oModel.MinAmount;
                        oProduct.Image = Convert.FromBase64String(oModel.Image);
                        oProduct.Required = oModel.Required;
                        oProduct.UnitAmount = oModel.UnitAmount; //Cantidad de elementos por paquete, generalmente 1
                        //oProduct.Expiration = oModel.Expiration;
                        oProduct.TypeAmount = oModel.TypeAmount;
                        oProduct.ProductAmount = oModel.ProductAmount;
                        if (oProduct.ProductAmount == 0)
                        {
                            return 2;
                        }

                        /*
                         This exception also happens if you don't use transaction properly. 
                        In my case, I put transaction.Commit() right after command.ExecuteReaderAsync(), 
                        did not wait with the transaction commiting until reader.ReadAsync() was called. The proper order:

                            Create transaction.
                            Create reader.
                            Read the data.
                            Commit the transaction.
                         */

                        var lstCompanies = await _context.ProductCompanies.Where((d) => d.IdProduct == oProduct.Id).ToListAsync();
                        foreach (var company in lstCompanies)
                        {
                            _context.Remove(company);
                        }
                        await _context.SaveChangesAsync();

                        /*
                        using (InventoryContext localContext = new InventoryContext())
                        {
                            foreach (var modelProdComp in oModel.ProductCompanies)
                            {
                                if (localContext.ProductCompanies.Where((d) => d.IdCompany == modelProdComp.Id && d.IdProduct == oProduct.Id).FirstOrDefault() == null)
                                {
                                    var prodComp = new Models.DB.ProductCompany();
                                    prodComp.IdCompany = modelProdComp.Id;
                                    prodComp.IdProduct = oProduct.Id;
                                    prodComp.Price = modelProdComp.Price;
                                    localContext.ProductCompanies.Add(prodComp);
                                    localContext.SaveChanges();
                                }
                            }
                        }
                        */

                        foreach (var modelProdComp in oModel.ProductCompanies)
                        {
                            if (_context.ProductCompanies.Where((d) => d.IdCompany == modelProdComp.Id && d.IdProduct == oProduct.Id).FirstOrDefault() == null)
                            {
                                var prodComp = new Models.DB.ProductCompany();
                                prodComp.IdCompany = modelProdComp.Id;
                                prodComp.IdProduct = oProduct.Id;
                                prodComp.Price = modelProdComp.Price;
                                _context.ProductCompanies.Add(prodComp);
                                _context.SaveChanges();
                            }
                        }




                        //Necesitamos asegurarnos que termina la insercion de companies ya que accederemos más tarde a estas
                        var companyAwait = _context.SaveChangesAsync();


                        var lstTags = await _context.ProductTags.Where((d) => d.IdProduct == oProduct.Id).ToListAsync();
                        foreach (var tag in lstTags)
                        {
                            _context.Remove(tag);
                        }
                        await _context.SaveChangesAsync();
                        foreach (var modelProdTag in oModel.ProductTags)
                        {
                            if (_context.ProductTags.Where((d) => d.IdTag == modelProdTag.Id && d.IdProduct == oProduct.Id).FirstOrDefault() == null)
                            {
                                var prodTag = new Models.DB.ProductTag();
                                prodTag.IdTag = modelProdTag.Id;
                                prodTag.IdProduct = oProduct.Id;
                                _context.ProductTags.Add(prodTag);
                                _context.SaveChanges();
                            }
                        }
                        companyAwait.Wait();

                        //Obtenemos los product Companies para saber el precio mínimo y la compania default
                        var lstNewCompanies = await _context.ProductCompanies.Where((d) => d.IdProduct == oProduct.Id).ToListAsync();

                        oProduct.Price = lstNewCompanies.Min(d => d.Price);
                        oProduct.DefaultCompany = lstNewCompanies.FirstOrDefault(d => d.Price == oProduct.Price)!.IdCompany;
                        oProduct.UnitPrice = oProduct.Price / oProduct.ProductAmount;

                        _context.Entry(oProduct).State = EntityState.Modified;
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
                if (!CompanyExists(id))
                {
                    return 1;
                }
                else
                {
                    throw;
                }
            }

        }

        public async Task<ActionResult<ProductRequest>> Get(long Id)
        {
            try
            {
                if (_context.Products == null)
                {
                    throw new Exception("No existe el campo de productos. Error en la base de datos.");
                }
                var oProduct = await _context.Products.FindAsync(Id);
                if (oProduct == null)
                {
                    throw new Exception("No existe el producto.");
                }

                ProductRequest oProductReq = new ProductRequest();

                /*
                var lstProductCompanies = _context.ProductCompanies.Where((d) => d.IdProduct == Id).ToList();
                var lstProductTags = _context.ProductTags.Where((d) => d.IdProduct == Id).ToList();
                var lstShoppinLists = _context.ShoppingLists.Where((d) => d.IdProduct == Id).ToList();
                */

                oProductReq.Id = oProduct.Id;
                oProductReq.Name = oProduct.Name;
                oProductReq.CurrentAmount = oProduct.CurrentAmount;
                oProductReq.MinAmount = oProduct.MinAmount;

                if (oProduct.Image != null)
                {
                    oProductReq.Image = Convert.ToBase64String(oProduct.Image);
                }
                else
                {

                    oProductReq.Image = "NO IMAGE";
                }
                oProductReq.Required = oProduct.Required;
                oProductReq.TypeAmount = oProduct.TypeAmount;
                oProductReq.ProductAmount = oProduct.ProductAmount;
                oProductReq.DefaultCompany = oProduct.DefaultCompany;
                oProductReq.Price = oProduct.Price;
                oProductReq.UnitAmount = oProduct.UnitAmount;
                //oProductReq.UnitPrice = oProduct.UnitPrice;


                var lstCompanies = _context.ProductCompanies.Where((d) => d.IdProduct == oProduct.Id).ToList();
                foreach (var prodComp in lstCompanies)
                {
                    var prodCompReq = new ProductCompany();
                    prodCompReq.Name = _context.Companies.Where((d) => d.Id == prodComp.IdCompany).First().Name;
                    prodCompReq.Id = prodComp.IdCompany;
                    prodCompReq.Price = prodComp.Price;
                    oProductReq.ProductCompanies.Add(prodCompReq);
                }


                var lstTags = _context.ProductTags.Where((d) => d.IdProduct == oProduct.Id).ToList();
                foreach (var prodTag in lstTags)
                {
                    var prodTagReq = new ProductTag();
                    prodTagReq.Name = _context.Tags.Where((d) => d.Id == prodTag.IdTag).First().Name;
                    prodTagReq.Id = prodTag.IdTag;
                    oProductReq.ProductTags.Add(prodTagReq);
                }


                return oProductReq;
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }

        public async Task<ActionResult<IEnumerable<Product>>> Get()
        {
            try
            {
                if (_context.Products == null)
                {
                    throw new Exception("No existe el campo de productos. Error en la base de datos.");
                }

                return await _context.Products.OrderByDescending(d => d.Id).ToListAsync();

                /*
                var prodId = (from d in _context.Products
                              where d.Name == modelName
                              select d.Id).First();

                if (prodId == null)
                {
                    throw new Exception("Nombre del producto facilitado no encontrado");
                }
                else
                {
                    return _context.Products.Find(prodId);
                }
                */
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }
        private bool CompanyExists(long id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public async Task<ActionResult<int>> Add(ProductRequest oModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Companies == null)
                    {
                        return 1;
                    }

                    var oProduct = new Product(); //Para modificar o añadir un objeto con varios campos aprender Automaper

                    //product.Total = oModel.Conceptos.Sum((d) => d.PrecioUnitario * d.Cantidad);
                    //Obtenemos precio unitario del frontend y los formularios de este, la otra forma seria de la base de datos,
                    //a la cual accederiamos mediante el id del producto, y obtendriamos el precio unitario de este

                    oProduct.Name = oModel.Name;
                    oProduct.CurrentAmount = oModel.CurrentAmount;
                    oProduct.MinAmount = oModel.MinAmount;
                    oProduct.Image = Convert.FromBase64String(oModel.Image);
                    oProduct.Required = oModel.Required; // Especial
                    oProduct.UnitAmount = oModel.UnitAmount; //Cantidad de elementos por paquete, generalmente 1
                    //oProduct.Expiration = oModel.Expiration;
                    oProduct.TypeAmount = oModel.TypeAmount;  //Tipo de producto (gramos, litros)
                    oProduct.ProductAmount = oModel.ProductAmount; //Cantidad x por tipo(gramos, litros)
                    if (oProduct.ProductAmount == 0)
                    {
                        return 2;
                    }


                    //Code to get min price with company priority > 0
                    /*
                    del2 del = (ProductCompany d) => {
                        var compPrio = (from f in _context.Companies
                                        where d.IdCompany == f.Id
                                        select f.Priority).FirstOrDefault();
                        return compPrio != null && compPrio != 0;
                    };

                    var idComp = oModel.ProductCompanies.Where((d) =>
                    {
                        foreach (var prodPrice in lstPrice)
                        {
                            return del(prodPrice);
                        }
                        return false;
                    }).FirstOrDefault().IdCompany;

                    var idComp = oModel.ProductCompanies.Where((d) =>
                    {
                        return d.Price == oProduct.Price && del(d);
                    }).FirstOrDefault().IdCompany;

                    oProduct.DefaultCompany = idComp != 0 ? idComp : 1;
                    */


                    //Datos default pero requeridos
                    oProduct.Price = 9999999999999;
                    oProduct.DefaultCompany = 1;
                    oProduct.UnitPrice = oProduct.Price / oProduct.ProductAmount;


                    _context.Products.Add(oProduct);
                    await _context.SaveChangesAsync();

                    foreach (var modelProdComp in oModel.ProductCompanies)
                    {
                        if (_context.ProductCompanies.Where((d) => d.IdCompany == modelProdComp.Id && d.IdProduct == oProduct.Id).FirstOrDefault() == null)
                        {
                            var prodComp = new Models.DB.ProductCompany();
                            prodComp.IdCompany = modelProdComp.Id;
                            prodComp.IdProduct = oProduct.Id;
                            prodComp.Price = modelProdComp.Price;
                            _context.ProductCompanies.Add(prodComp);
                            _context.SaveChanges();
                        }
                    }
                    var companyAwait = _context.SaveChangesAsync();
                    foreach (var modelProdTag in oModel.ProductTags)
                    {
                        if (_context.ProductTags.Where((d) => d.IdTag == modelProdTag.Id && d.IdProduct == oProduct.Id).FirstOrDefault() == null)
                        {
                            var prodTag = new Models.DB.ProductTag();
                            prodTag.IdTag = modelProdTag.Id;
                            prodTag.IdProduct = oProduct.Id;
                            _context.ProductTags.Add(prodTag);
                            _context.SaveChanges();
                        }
                    }
                    companyAwait.Wait();

                    //Obtenemos los product Companies para saber el precio mínimo y la compania default
                    var lstCompanies = await _context.ProductCompanies.Where((d) => d.IdProduct == oProduct.Id).ToListAsync();

                    Product oProductInDB = _context.Products.Find(oProduct.Id)!;
                    _context.Entry(oProductInDB).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    oProductInDB.Price = lstCompanies.Min(d => d.Price);
                    oProductInDB.DefaultCompany = lstCompanies.FirstOrDefault(d => d.Price == oProductInDB.Price)!.IdCompany;
                    oProductInDB.UnitPrice = oProductInDB.Price / oProductInDB.ProductAmount;

                    //await _context.SaveChangesAsync();
                    _context.SaveChanges();

                    transaction.Commit();
                    return 0;

                    //Ejemplo de forzar el Rollback:
                    //if(false) throw new Exception(); y YATA =)
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Ocurrió un error en la inserción");
                }
            }
        }
        public async void Edit(ProductRequest oModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var prodId = (from d in _context.Products
                                  where d.Name == oModel.Name
                                  select d.Id).First();

                    Product oProduct = _context.Products.Find(prodId)!;


                    oProduct.Name = oModel.Name;
                    oProduct.CurrentAmount = oModel.CurrentAmount;
                    oProduct.MinAmount = oModel.MinAmount;
                    oProduct.Image = Convert.FromBase64String(oModel.Image);


                    //Obtenemos lista de companies, eliminamos las companies y metemos las nuevas que nos han dado
                    var lstCompanies = (from d in _context.ProductCompanies
                                        where d.IdProduct == oProduct.Id
                                        select d).ToList();
                    foreach (var company in lstCompanies)
                    {
                        _context.Remove(company);
                    }

                    await _context.SaveChangesAsync();
                    foreach (var modelProdComp in oModel.ProductCompanies)
                    {
                        if (_context.ProductCompanies.Where((d) => d.IdCompany == modelProdComp.Id && d.IdProduct == oProduct.Id).First() != null)
                        {
                            var prodComp = new Models.DB.ProductCompany();
                            prodComp.IdCompany = modelProdComp.Id;
                            prodComp.IdProduct = oProduct.Id;
                            prodComp.Price = modelProdComp.Price;
                            _context.ProductCompanies.Add(prodComp);
                            _context.SaveChanges();
                        }
                    }
                    var companyAwait = _context.SaveChangesAsync();



                    //Obtenemos lista de tags, eliminamos los tags y metemos los nuevos que nos han dado
                    var lstTags = (from d in _context.ProductTags
                                   where d.IdProduct == oProduct.Id
                                   select d).ToList();
                    foreach (var tag in lstTags)
                    {
                        _context.Remove(tag);
                    }
                    await _context.SaveChangesAsync();
                    foreach (var modelProdTag in oModel.ProductTags)
                    {
                        if (_context.ProductTags.Where((d) => d.IdTag == modelProdTag.Id && d.IdProduct == oProduct.Id).First() != null)
                        {
                            var prodTag = new Models.DB.ProductTag();
                            prodTag.IdTag = modelProdTag.Id;
                            prodTag.IdProduct = oProduct.Id;
                            _context.ProductTags.Add(prodTag);
                            _context.SaveChanges();
                        }
                    }


                    //Obtenemos los product Companies para saber el precio mínimo y la compania default
                    companyAwait.Wait();
                    var lstNewCompanies = (from d in _context.ProductCompanies
                                           where d.IdProduct == oProduct.Id
                                           select d).ToList();

                    oProduct.Price = lstNewCompanies.Min(d => d.Price);

                    oProduct.DefaultCompany = lstNewCompanies.FirstOrDefault(d => d.Price == oProduct.Price)!.IdCompany;

                    oProduct.ProductAmount = oModel.ProductAmount;
                    oProduct.UnitPrice = oProduct.Price / oProduct.ProductAmount;

                    _context.Entry(oProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.SaveChanges();

                    transaction.Commit();

                    //Ejemplo de forzar el Rollback:
                    //if(false) throw new Exception(); y YATA =)
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw new Exception("Ocurrió un error en la edición del producto");
                }
            }
        }

        /*
        public Product Get(string modelName)
        {
            try
            {
                var prodId = (from d in _context.Products
                              where d.Name == modelName
                              select d.Id).First();

                if (prodId == null)
                {
                    throw new Exception("Nombre del producto facilitado no encontrado");
                }
                else
                {
                    return _context.Products.Find(prodId);
                    //ProductRequest oModel = new ProductRequest();


                    //product.Total = oModel.Conceptos.Sum((d) => d.PrecioUnitario * d.Cantidad);
                    //Obtenemos precio unitario del frontend y los formularios de este, la otra forma seria de la base de datos,
                    //a la cual accederiamos mediante el id del producto, y obtendriamos el precio unitario de este


                    /*
                oModel.Name = oProduct.Name;
                oModel.CurrentAmount = oProduct.CurrentAmount;
                oModel.MinAmount = oProduct.MinAmount;
                oModel.Image = oProduct.Image;
                oModel.Price = oProduct.Price;
                oModel.DefaultCompany = oProduct.DefaultCompany;
                oModel.Required = oProduct.Required;
                oModel.Expiration = oProduct.Expiration;
                oModel.ProductAmount = oProduct.ProductAmount;
                oModel.UnitPrice = oProduct.UnitPrice;
                oModel.TypeAmount = oProduct.TypeAmount;

                foreach (var prodComp in oProduct.ProductCompanies)
                {
                        var modelProdComp = new ProductCompany();
                        modelProdComp.IdCompany = prodComp.IdCompany;
                        modelProdComp.Price = prodComp.Price;
                }
                */


                    //Ejemplo de forzar el Rollback:
                    //if(false) throw new Exception(); y YATA =)
                    /*
                }
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la inserción");
            }
        }

*/
        public async void Reload(long Id)
        {
            try
            {
                Product oProduct = _context.Products.Find(Id)!;

                var lstCompanies = (from d in _context.ProductCompanies
                                    where d.IdProduct == oProduct.Id
                                    select d).ToList();
                /*
                foreach (var company in lstCompanies)
                {
                    _context.Remove(company);
                }
                foreach (var modelProdComp in lstCompanies)
                {
                    var prodComp = new Models.DB.ProductCompany();
                    prodComp.IdCompany = modelProdComp.IdCompany;
                    prodComp.IdProduct = oProduct.Id;
                    prodComp.Price = modelProdComp.Price;
                    _context.ProductCompanies.Add(prodComp);
                }
                await _context.SaveChangesAsync();
                */

                oProduct.Price = lstCompanies.Min(d => d.Price);
                oProduct.DefaultCompany = lstCompanies.FirstOrDefault(d => d.Price == oProduct.Price)!.IdCompany;


                oProduct.ProductAmount = oProduct.ProductAmount;
                oProduct.UnitPrice = oProduct.Price / oProduct.ProductAmount;
                /*
                var lstTags = (from d in _context.ProductTags
                               where d.IdProduct == oProduct.Id
                               select d).ToList();
                foreach (var tag in lstTags)
                {
                    _context.Remove(tag);
                }
                foreach (var modelProdTag in lstTags)
                {
                    var prodTag = new Models.DB.ProductTag();
                    prodTag.IdTag = modelProdTag.IdTag;
                    prodTag.IdProduct = oProduct.Id;
                    _context.ProductTags.Add(prodTag);
                }
                */

                _context.Entry(oProduct).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _context.SaveChangesAsync();
                //await _context.SaveChangesAsync();


                //Ejemplo de forzar el Rollback:
                //if(false) throw new Exception(); y YATA =)
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la inserción");
            }
        }
    }
}
