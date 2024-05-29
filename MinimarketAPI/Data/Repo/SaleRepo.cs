using Microsoft.EntityFrameworkCore;

using Minimarket.Data.Context;
using Minimarket.Model;

namespace Minimarket.Data.Repo;

public class SaleRepo : GenericRepo<Sale>, ISaleRepo
{
    private readonly MinimarketContext _context;

    public SaleRepo(MinimarketContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Sale> Register(Sale model)
    {
        Sale generateSale = new();
        using (var transaction = _context.Database.BeginTransaction())
        {
            void TransactionFail(string msg = "Falló el registro")
            {
                transaction.Rollback();
                throw new Exception(msg);
            }

            foreach (SaleDetail sd in model.SaleDetails)
            {
                var product = await _context.Products.Where(
                    p => p.Id == sd.IdProduct)
                .FirstOrDefaultAsync()
                ?? throw new TaskCanceledException("Producto no encontrado");
                
                product!.Stock -= sd.Amount;
                _context.Products.Update(product);
            }
            if (await _context.SaveChangesAsync() < 1)
            {
                TransactionFail("Falló al ingresar detalles de venta");
            }

            var correlative = await _context.DocumentNumbers.FirstOrDefaultAsync();
            if (correlative == null)
            {
                correlative = new DocumentNumber()
                {
                    LastNumber = 0000,
                    RegisterDate = DateTime.Now
                };
                await _context.SaveChangesAsync();
            }

            correlative!.LastNumber += 1;
            correlative.RegisterDate = DateTime.Now;
            _context.DocumentNumbers.Update(correlative);
            if (await _context.SaveChangesAsync() < 1)
            {
                TransactionFail("Falló ingresar fecha de registro");
            }

            int characters = 4;
            string zeros = string.Concat(Enumerable.Repeat("0", characters));
            string saleNumber = zeros + correlative.LastNumber.ToString();
            saleNumber = saleNumber.Substring(saleNumber.Length - characters, characters);
            model.DocumentNumber = saleNumber;
            await _context.Sales.AddAsync(model);

            if (await _context.SaveChangesAsync() < 1)
            {
                TransactionFail("Falló ingresar número de documento");
            }
            generateSale = model;
            transaction.Commit();
        }
        return generateSale;
    }
}