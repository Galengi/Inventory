using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class CompanyService: ICompanyService
    {
        delegate bool del2(ProductCompany d);
        private readonly InventoryContext _context;
        private IProductService _product;

        public CompanyService(InventoryContext context, IProductService product)
        {
            this._product = product;
            this._context = context;
        }
        public async Task<ActionResult<int>> Add(CompanyRequest oModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Companies == null)
                    {
                        return 1;
                    }

                    Company oCompany = new Company();

                    oCompany.Name = oModel.Name;
                    oCompany.Priority = oModel.Priority;

                    _context.Companies.Add(oCompany);
                    await _context.SaveChangesAsync();

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

        public async Task<ActionResult<int>> Delete(long Id)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Companies == null)
                    {
                        throw new Exception("No existe el campo de empresas. Error en la base de datos.");
                    }
                    var oCompany = await _context.Companies.FindAsync(Id);
                    if (oCompany == null)
                    {
                        throw new Exception("No existe la empresa.");
                    }

                    List<long> lstProductsId = new List<long>();
                    var lstProducts = (from d in _context.ProductCompanies
                                       where d.IdCompany == Id
                                       select d).ToList();
                    foreach (var product in lstProducts)
                    {
                        if (!lstProductsId.Contains(product.IdProduct))
                        {
                            lstProductsId.Add(product.IdProduct);
                        }
                        _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.ProductCompanies.Remove(product);
                    }
                    await _context.SaveChangesAsync();
                    foreach (var productId in lstProductsId)
                    {
                        _product.Reload(productId);
                    }

                    await _context.SaveChangesAsync();

                    //////////////////////////////////////////////
                    _context.Entry(oCompany).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.Companies.Remove(oCompany);
                    //_context.Remove(company);
                    //////////////////////////////////////////
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

        public async Task<ActionResult<int>> Edit(long id, CompanyRequest oModel)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Company oCompany = _context.Companies.Find(id)!;

                        oCompany.Name = oModel.Name;
                        oCompany.Priority = oModel.Priority;


                        _context.Entry(oCompany).State = EntityState.Modified;
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

        public async Task<ActionResult<CompanyRequest>> Get(long Id)
        {
            try
            {
                if (_context.Companies == null)
                {
                    throw new Exception("No existe el campo de empresas. Erro en la base de datos.");
                }
                var oCompany = await _context.Companies.FindAsync(Id);
                if (oCompany == null)
                {
                    throw new Exception("No existe la empresa.");
                }

                CompanyRequest oCompanyReq = new CompanyRequest();
                oCompanyReq.Name = oCompany.Name;
                oCompanyReq.Priority = oCompany.Priority;

                List<ProductRequest> lstProdReq = new List<ProductRequest>();

                var lstProductCompanies = _context.ProductCompanies.Where((d) => d.IdCompany == Id).ToList();

                foreach (var productCompany in lstProductCompanies)
                {
                    ProductRequest prodReq = _product.Get(productCompany.IdProduct).Result.Value!;
                    if (prodReq != null && !lstProdReq.Contains(prodReq))
                    {
                        lstProdReq.Add(prodReq);
                    }
                }
                oCompanyReq.ProductModels = lstProdReq;




                /*
                var compReq = new CompanyRequest();

                compReq.Name = oCompany.Name;
                compReq.Priority = oCompany.Priority;

                var lstProduct = _context.ProductCompanies.Where((d) => d.IdCompany == Id).ToList();
                foreach (var prod in lstProduct)
                {
                    compReq.Products.Add(_context.Products.Where((d) => d.Id == prod.IdProduct).First());
                }
                */
                /*

                var lstProductCompanies = _context.ProductCompanies.Where((d) => d.IdCompany == Id).ToList();
                var lstProduct = _context.Products.Where((d) => d.DefaultCompany == Id).ToList();
                var lstShoppinLists = _context.ShoppingLists.Where((d) => d.IdCompany == Id).ToList();
                */
                /*
                foreach (var prod in lstProduct)
                {
                    oCompany.Products.Add(_context.Products.Where((d) => d.Id == prod.IdProduct).First());
                }
                */

                return oCompanyReq;

                /*
                var prodId = (from d in db.Products
                              where d.Name == modelName
                              select d.Id).First();

                if (prodId == null)
                {
                    throw new Exception("Nombre del producto facilitado no encontrado");
                }
                else
                {
                    return db.Products.Find(prodId);
                }
                */
            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }

        public async Task<ActionResult<IEnumerable<Company>>> Get()
        {
            try
            {
                if (_context.Companies == null)
                {
                    throw new Exception("No existe el campo de empresas. Error en la base de datos.");
                }

                return await _context.Companies.ToListAsync();

                /*
                var prodId = (from d in db.Products
                              where d.Name == modelName
                              select d.Id).First();

                if (prodId == null)
                {
                    throw new Exception("Nombre del producto facilitado no encontrado");
                }
                else
                {
                    return db.Products.Find(prodId);
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
    }
}
