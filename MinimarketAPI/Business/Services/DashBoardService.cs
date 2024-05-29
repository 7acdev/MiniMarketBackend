using System.Globalization;
using AutoMapper;
using Minimarket.Data.Repo;
using Minimarket.DTO;
using Minimarket.Model;

namespace Minimarket.Business.Service;

public class DashBoardService : IDashBoardService
{
    private readonly ISaleRepo _saleRepo;
    private readonly IGenericRepo<Product> _productRepo;

    public DashBoardService(ISaleRepo saleRepo,
        IGenericRepo<Product> productRepo, IMapper mapper)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
    }

    private static IQueryable<Sale> GetSales(IQueryable<Sale> saleTable, int substractDays)
    {
        DateTime? last = saleTable.OrderByDescending(s => s.RegisterDate).Select(s => s.RegisterDate).First();
        last = last!.Value.AddDays(substractDays);
        return saleTable.Where(s => s.RegisterDate!.Value.Date >= last.Value.Date);
    }

    private int TotalSalesLastWeek()
    {
        int total = 0;
        IQueryable<Sale> _saleQuery = _saleRepo.Query();

        if (_saleQuery.Count() > 0)
        {
            var saleTable = GetSales(_saleQuery, -7);
            total = saleTable.Count();
        }
        return total;
    }

    private string TotalRenueveLastWeek()
    {
        decimal res = 0;
        IQueryable<Sale> saleQuery = _saleRepo.Query();

        if (saleQuery.Count() > 0)
        {
            var saleTable = GetSales(saleQuery, -7);
            res = saleTable.Select(s => s.Total).Sum(s => s!.Value);
        }
        return Convert.ToString(res, CultureInfo.InvariantCulture);
    }

    private int TotalProducts()
    {
        IQueryable<Product> query = _productRepo.Query();
        return query.Count();
    }

    private Dictionary<string, int> SalesLastWeek()
    {
        Dictionary<string, int> res = new();
        IQueryable<Sale> query = _saleRepo.Query();

        if (query.Count() > 0)
        {
            var saleTable = GetSales(query, -7);

            res = saleTable
            .GroupBy(s => s.RegisterDate!.Value.Date).OrderBy(g => g.Key)
            .Select(ds => new { date = ds.Key.ToString("dd/MM/yyyy"), total = ds.Count() })
            .ToDictionary(keySelector: s => s.date, elementSelector: s => s.total);
        }
        return res;
    }

    public DashBoardDTO Summary()
    {
        DashBoardDTO dash = new();
        try
        {
            dash.TotalSales = TotalSalesLastWeek();
            dash.TotalRevenue = TotalRenueveLastWeek();
            dash.TotalProducts = TotalProducts();

            List<SaleWeekDTO> salesWeek = new();
            foreach (KeyValuePair<string, int> item in SalesLastWeek())
            {
                salesWeek.Add(new SaleWeekDTO()
                {
                    Date = item.Key,
                    Total = item.Value
                });
            }
            dash.LastWeekSales = salesWeek;
        }
        catch
        {
            throw;
        }
        return dash;
    }
}