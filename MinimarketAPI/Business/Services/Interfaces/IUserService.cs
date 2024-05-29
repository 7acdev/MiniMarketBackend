using Microsoft.AspNetCore.Http;
using Minimarket.DTO;

namespace Minimarket.Business.Service;

public interface IUserService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<ResponseUserDTO> Create(UserDTO model, HttpContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<bool> Confirm(string token);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<SessionDTO> Auth(LoginDTO model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<bool> Edit(UserDTO model, HttpContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<bool> Delete(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResponseUserDTO> GetUser(int id);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <param name="seach"></param>
    /// <returns></returns>
    Task<List<ResponseUserDTO>> UserList(string role, string seach);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    Task<bool> ResetPassword(string email);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<bool> NewPassword(ResetPasswordDTO model);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<ResponseUserDTO> Profile(HttpContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="upload"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    Task<string> UploadProfileImage(IFormFile upload, HttpContext context);
}