using Inventory.Models.DB;
using Inventory.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services
{
    public class TypeService : ITypeService
    {
        private readonly InventoryContext _context;

        public TypeService(InventoryContext context)
        {
            this._context = context;
        }
        public async Task<ActionResult<int>> Add(TypeRequest oModel)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (_context.TypeUnits == null)
                    {
                        return 1;
                    }

                    TypeUnit oType = new TypeUnit();

                    oType.Name = oModel.Name;

                    _context.TypeUnits.Add(oType);
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

                    if (_context.TypeUnits == null)
                    {
                        throw new Exception("No existe el campo de tipos de unidad. Error en la base de datos.");
                    }
                    var oType = await _context.TypeUnits.FindAsync(Id);
                    if (oType == null)
                    {
                        throw new Exception("No existe el tipo de unidad.");
                    }

                    var lstProductsId = new List<long>();
                    var lstProducts = _context.Products.Where(d => d.TypeAmount == Id).ToList();
                    foreach (var product in lstProducts)
                    {
                        if (!lstProductsId.Contains(product.Id))
                        {
                            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            product.TypeAmount = 0;
                            lstProductsId.Add(product.Id);
                        }
                    }

                    await _context.SaveChangesAsync();


                    _context.Entry(oType).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _context.TypeUnits.Remove(oType);

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

        public async Task<ActionResult<int>> Edit(long id, TypeRequest oModel)
        {
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        TypeUnit? oType = _context.TypeUnits.Find(id);
                        if (oType == null)
                        {
                            throw new Exception("No existe el tipo de unidad.");
                        }

                        oType.Name = oModel.Name;


                        _context.Entry(oType).State = EntityState.Modified;
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
                if (!TypeExists(id))
                {
                    return 1;
                }
                else
                {
                    throw;
                }
            }

        }

        public async Task<ActionResult<TypeUnit>> Get(long Id)
        {
            try
            {


                if (_context.TypeUnits == null)
                {
                    throw new Exception("No existe el campo de tipos de unidad. Error en la base de datos.");
                }
                var oType = await _context.TypeUnits.FindAsync(Id);
                if (oType == null)
                {
                    throw new Exception("No existe el tipo de unidad.");

                }

                return oType;

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }

        public async Task<ActionResult<IEnumerable<TypeUnit>>> Get()
        {
            try
            {

                if (_context.TypeUnits == null)
                {
                    throw new Exception("No existe el campo de tipos de unidad. Error en la base de datos.");
                }

                return await _context.TypeUnits.ToListAsync();

            }
            catch (Exception)
            {
                throw new Exception("Ocurrió un error en la obtención");
            }
        }
        private bool TypeExists(long id)
        {
            return (_context.TypeUnits?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
