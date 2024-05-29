using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minimarket.Data.Repo;
using Minimarket.DTO;
using Minimarket.Model;

namespace Minimarket.Business.Service;

public class SaleService : ISaleService
{
    private readonly IGenericRepo<SaleDetail> _saleDetailRepo;
    private readonly ISaleRepo _repo;
    private readonly IMapper _mapper;

    public SaleService(IGenericRepo<SaleDetail> saleDetailRepo, ISaleRepo repo, IMapper mapper)
    {
        _saleDetailRepo = saleDetailRepo;
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<SaleDTO> Register(SaleDTO model)
    {
        var sale = await _repo.Register(_mapper.Map<Sale>(model));
        if (sale.Id == 0)
            throw new TaskCanceledException("No se pudo registrar la venta");

        return _mapper.Map<SaleDTO>(sale);
    }

    public async Task<List<SaleDTO>> History(string seach, string saleNumber, string startDate, string endDate)
    {
        var query = _repo.Query() ?? throw new TaskCanceledException("Historial no encontrado");

        List<Sale> list = new();
        if (seach.ToLower() == "fecha")
        {
            DateTime start = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
            DateTime end = DateTime.ParseExact(endDate, "d/M/yyyy", CultureInfo.InvariantCulture);

            list = await query.Where(v => v.RegisterDate!.Value.Date >= start &&
                v.RegisterDate.Value.Date <= end.Date
            )
            .Include(d => d.SaleDetails)
            .ThenInclude(p => p.IdProductNavigation)
            .ToListAsync();

            if (list.Count == 0)
                throw new TaskCanceledException("Venta no encontrada");

        }
        else
        {
            list = await query.Where(v => v.DocumentNumber == saleNumber)
            .Include(d => d.SaleDetails)
            .ThenInclude(p => p.IdProductNavigation)
            .ToListAsync();

            if (list.Count == 0)
                throw new TaskCanceledException("Venta no encontrada");

        }
        return _mapper.Map<List<SaleDTO>>(list);
    }

    public async Task<List<ReportDTO>> Report(string startDate, string endDate)
    {
        var query = _saleDetailRepo.Query() ?? throw new TaskCanceledException("Detalles de venta no encontrados");

        List<SaleDetail> list = new();

        DateTime start = DateTime.ParseExact(startDate, "d/M/yyyy", CultureInfo.InvariantCulture);
        DateTime end = DateTime.ParseExact(endDate, "d/M/yyyy", CultureInfo.InvariantCulture);

        list = await query
        .Include(p => p.IdProductNavigation)
        .Include(s => s.IdSaleNavigation)
        .Where(sd =>
            sd.IdSaleNavigation!.RegisterDate!.Value.Date >= start.Date &&
            sd.IdSaleNavigation.RegisterDate.Value.Date <= end.Date
        )
        .ToListAsync();

        return _mapper.Map<List<ReportDTO>>(list);
    }
}