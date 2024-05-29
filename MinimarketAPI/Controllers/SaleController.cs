using Microsoft.AspNetCore.Mvc;
using Minimarket.Business.Service;
using Minimarket.DTO;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SaleController : ControllerBase
{
    private readonly ISaleService _service;

    public SaleController(ISaleService service)
    {
        _service = service;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] SaleDTO sale)
    {
        var res = new ResponseDTO<SaleDTO>();
        try
        {
            res.Result = await _service.Register(sale);
            res.Success = true;
            res.Message = "Venta registrada correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [HttpGet("History")]
    public async Task<IActionResult> History(
        string seach, string? saleNumber = "NA", string? startDate = "NA", string? endDate = "NA")
    {
        saleNumber = saleNumber == "NA" ? "" : saleNumber;
        startDate = startDate == "NA" ? "" : startDate;
        endDate = endDate == "NA" ? "" : endDate;

        var res = new ResponseDTO<List<SaleDTO>>();
        try
        {
            res.Result = await _service.History(seach, saleNumber!, startDate!, endDate!);
            res.Success = true;
            res.Message = "Historial obtenido correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [HttpGet("Report")]
    public async Task<IActionResult> Report(string startDate, string endDate)
    {
        ResponseDTO<List<ReportDTO>> res = new();
        try
        {
            res.Result = await _service.Report(startDate, endDate);
            res.Success = true;
            res.Message = "Reporte obtenido correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }
}