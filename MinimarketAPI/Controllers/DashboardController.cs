using Microsoft.AspNetCore.Mvc;
using Minimarket.Business.Service;
using Minimarket.DTO;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashBoardService _dashService;

    public DashboardController(IDashBoardService dashService)
    {
        _dashService = dashService;
    }

    [HttpGet("Summary")]
    public IActionResult Summary()
    {
        var res = new ResponseDTO<DashBoardDTO>();
        try
        {
            res.Result = _dashService.Summary();
            res.Success = true;
            res.Message = "Resumen obtenido correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }
}