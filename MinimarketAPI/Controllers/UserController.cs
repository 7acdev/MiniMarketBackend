using Microsoft.AspNetCore.Mvc;
using Minimarket.Business.Service;
using Minimarket.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Minimarket.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    #region Creation
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] UserDTO model)
    {
        var res = new ResponseDTO<ResponseUserDTO>();
        try
        {
            res.Result = await _userService.Create(model, HttpContext);
            res.Success = true;
            res.Message = "Usuario Creado Correctamente";
            
            // NOTA: Actualmente no usamos un email de confirmación.
            //res.Message = "Usuario Creado Correctamente te envíamos un email de confirmación";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [HttpGet("Confirm/{token}")]
    public async Task<IActionResult> Confirm(string token)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _userService.Confirm(token);
            res.Success = true;
            res.Message = "Su correo electronico fue confirmado correctamente";
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
            res.Result = await _userService.Delete(id);
            res.Success = true;
            res.Message = "Usuario eliminado correctamente";
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
    public async Task<IActionResult> Edit([FromBody] UserDTO model)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _userService.Edit(model, HttpContext);
            res.Success = true;
            res.Message = "Usuario editado correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    #endregion

    #region Login
    
    [HttpPost("Auth")]
    public async Task<IActionResult> Auth([FromBody] LoginDTO model)
    {
        var res = new ResponseDTO<SessionDTO>();
        try
        {
            res.Result = await _userService.Auth(model);
            res.Success = true;
            res.Message = "Sesión iniciada correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        var res = new ResponseDTO<ResponseUserDTO>();
        try
        {
            var context = HttpContext;
            res.Result = await _userService.Profile(context);
            res.Success = true;
            res.Message = "Perfil obtenido con exito";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    #endregion

    #region Info

    [Authorize]
    [HttpGet("GetUser/{id:int}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var res = new ResponseDTO<ResponseUserDTO>();
        try
        {
            res.Result = await _userService.GetUser(id);
            res.Success = true;
            res.Message = "Usuario Obtenido con exito";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [Authorize]
    [HttpGet("UserList/{role:alpha}/{seach:alpha?}")]
    public async Task<IActionResult> UserList(string role, string seach = "NA")
    {
        var res = new ResponseDTO<List<ResponseUserDTO>>();
        try
        {
            if (seach == "NA")
                seach = "";

            res.Result = await _userService.UserList(role, seach);
            res.Success = true;
            res.Message = "Usuarios obtenidos con exito";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    #endregion

    #region Recuperar cuenta
   
    [HttpPost("ResetPassword/{email}")]
    public async Task<IActionResult> ResetPassword(string email)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _userService.ResetPassword(email);
            res.Success = true;
            res.Message = "Envíamos un token de confirmación a su correo electronico";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    [HttpPost("NewPassword")]
    public async Task<IActionResult> NewPassword([FromBody] ResetPasswordDTO model)
    {
        var res = new ResponseDTO<bool>();
        try
        {
            res.Result = await _userService.NewPassword(model);
            res.Success = true;
            res.Message = "Password reseteado correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    #endregion

    #region Imagenes

    [Authorize]
    [RequestFormLimits(MultipartBodyLengthLimit = 2000000)]
    [HttpPost("UploadProfileImage")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        var res = new ResponseDTO<string>();
        try
        {
            res.Result = await _userService.UploadProfileImage(image, HttpContext);
            res.Success = true;
            res.Message = "Imagen subida correctamente";
        }
        catch (Exception ex)
        {
            res.Success = false;
            res.Message = ex.Message;
        }
        return Ok(res);
    }

    #endregion
}