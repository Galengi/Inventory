using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class TagService: ITagService
    {
        delegate bool del2(ProductTag d);
        private readonly InventoryContext _context;
        private IProductService _product;

        public TagService(InventoryContext context, IProductService product)
        {
            this._product = product;
            this._context = context;
        }
        public async Task<ActionResult<int>> Add(TagRequest oModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.Tags == null)
                    {
                        return 1;
                    }

                    Tag oTag = new Tag();

                    oTag.Name = oModel.Name;
                    oTag.Priority = oModel.Priority;

                    _context.Tags.Add(oTag);
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

                    if (_context.Tags == null)
                    {
                        throw new Exception("No existe el campo de tags. Error en la base de datos.");
                    }
                    var tag = await _context.Tags.FindAsync(Id);
                    if (tag == null)
                    {
                        throw new Exception("No existe el tag.");
                    }
                    var lstProducts = (from d in _context.ProductTags
                                       where d.IdTag == Id
                                       select d).ToList();
                    foreach (var product in lstProducts)
                    {
                        _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _context.ProductTags.Remove(product);
                    }
                    await _context.SaveChangesAsync();

                    //////////////////////////////////////////////
                    _context.Entry(tag).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.Tags.Remove(tag);
                    //_context.Remove(tag);
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

        public async Task<ActionResult<int>> Edit(long id, TagRequest oModel)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        Tag? oTag = _context.Tags.Find(id);

                        if (oTag == null)
                        {
                            throw new Exception("No existe el tag.");
                        }

                        oTag.Name = oModel.Name;
                        oTag.Priority = oModel.Priority;


                        _context.Entry(oTag).State = EntityState.Modified;
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
                if (!TagExists(id))
                {
                    return 1;
                }
                else
                {
                    throw;
                }
            }

        }

        public async Task<ActionResult<TagRequest>> Get(long Id)
        {
            try
            {

                if (_context.Tags == null)
                {
                    throw new Exception("No existe el campo de tags. Error en la base de datos.");
                }
                var oTag = await _context.Tags.FindAsync(Id);
                if (oTag == null)
                {
                    throw new Exception("No existe el tag.");
                }

                TagRequest oTagReq = new TagRequest();
                oTagReq.Name = oTag.Name;
                oTagReq.Priority = oTag.Priority;
                List<ProductRequest> lstProdReq = new List<ProductRequest>();

                var lstProductTags = _context.ProductTags.Where((d) => d.IdTag == Id).ToList();

                foreach (var productTag in lstProductTags)
                {
                    ProductRequest prodReq = _product.Get(productTag.IdProduct).Result.Value!;
                    if (prodReq != null && !lstProdReq.Contains(prodReq))
                    {
                        lstProdReq.Add(prodReq);
                    }
                }
                oTagReq.ProductModels = lstProdReq;


                /*

                var tagReq = new TagRequest();

                tagReq.Name = oTag.Name;
                tagReq.Priority = oTag.Priority;

                var lstProduct = _context.ProductTags.Where((d) => d.IdTag == Id).ToList();
                foreach (var prod in lstProduct)
                {
                    tagReq.Products.Add(_context.Products.Where((d)=> d.Id == prod.IdProduct).First());
                }
                */


                //oTag.ProductTags = _context.ProductTags.Where((d) => d.IdTag == Id).ToList();

                return oTagReq;

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

        public async Task<ActionResult<IEnumerable<Tag>>> Get()
        {
            try
            {
                if (_context.Tags == null)
                {
                    throw new Exception("No existe el campo de tags. Error en la base de datos.");
                }

                return await _context.Tags.ToListAsync();

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
        private bool TagExists(long id)
        {
            return (_context.Tags?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
