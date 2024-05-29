using Microsoft.AspNetCore.Mvc;
using Minimarket.Business.Service;
using Minimarket.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] ProductDTO model)
    {
        var res = new ResponseDTO<ProductDTO>();
        try
        {
            res.Result = await _productService.Create(model);
            res.Success = true;
            res.Message = "Producto Creado Correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpDelete("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _productService.Delete(id);
            res.Success = true;
            res.Message = "Usuario Eliminado Correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpPut("Edit")]
    public async Task<IActionResult> Edit([FromBody] ProductDTO model)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _productService.Edit(model);
            res.Success = true;
            res.Message = "Producto editado correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpGet("GetProduct/{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var res = new ResponseDTO<ProductDTO>();
        try
        {
            res.Result = await _productService.GetProduct(id);
            res.Success = true;
            res.Message = "Usuario Obtenido Correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpGet("ProductList/{seach:alpha?}")]
    public async Task<IActionResult> ProductList(string seach = "NA")
    {
        var res = new ResponseDTO<List<ProductDTO>>();
        try
        {
            if (seach == "NA")
                seach = "";

            res.Result = await _productService.ProductList(seach);
            res.Success = true;
            res.Message = "Productos Obtenidos Correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }
}