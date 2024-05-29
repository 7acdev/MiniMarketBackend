using AutoMapper;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Minimarket.Data.Repo;
using Minimarket.DTO;
using Minimarket.Model;
using Minimarket.Util;
using System.Linq.Expressions;
using System.Reflection;


namespace Minimarket.Business.Service;

public class UserService : IUserService
{
    private readonly IGenericRepo<Usuario> _repoModel;
    private readonly IMapper _mapper;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _enviro;

    public UserService(
        IGenericRepo<Usuario> repoModel, IMapper mapper,
        IConfiguration config, IWebHostEnvironment enviro
    )
    {
        _repoModel = repoModel;
        _mapper = mapper;
        _config = config;
        _enviro = enviro;
    }

    // Create
    //---------------------------------------------------------------------

    private async Task<ResponseUserDTO> Create(UserDTO model, bool byAdmin = false)
    {
        if (model.Password == "")
        {
            throw new TaskCanceledException("Password Vacío");
        }

        // Comprobar el rol.
        if (!(model.Role == UserRoles.admin || model.Role == UserRoles.employee))
        {
            throw new TaskCanceledException(
                $"El rol no existe, los roles disponibles son: {UserRoles.admin} o {UserRoles.employee}"
            );
        }

        // Necesitamos el Id para modificar al usuario pero no para crear uno nuevo.
        model.Id = null;

        // NOTA: Si aún no se ha confirmado podríamos permitir un nuevo registro y eliminar el correo no confirmado cuando
        // uno de los dos se confirme. También añadir un plazo de tiempo para eliminar el correo no confirmado.

        var query = await _repoModel.Query(p => p.Email!.ToLower() == model.Email!.ToLower()).FirstOrDefaultAsync();
        if (query != null)
        {
            throw new TaskCanceledException("El correo electronico ya esta registrado");
        }
        var dbModel = _mapper.Map<Usuario>(model);

        if (!byAdmin)
        {
            dbModel.Token = TokenGenerator.GenerateToken();
            dbModel.IsConfirmed = false;
        }
        else
        {
            dbModel.IsConfirmed = true;
        }
        dbModel.Password = HashPassword(dbModel.Password!);

        var createdModel = await _repoModel.Create(_mapper.Map<Usuario>(dbModel));
        if (createdModel.Id != 0)
        {
            return _mapper.Map<ResponseUserDTO>(createdModel);
        }
        else
        {
            throw new TaskCanceledException("Imposible crear, ID es igual a cero");
        }
    }

    private static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt
            .EnhancedHashPassword(password, BCrypt.Net.HashType.SHA384, 11);
    }

    // NOTA: Para pruebas permitimos que todos los usuarios puedan crear un admin.
    public async Task<ResponseUserDTO> Create(UserDTO model, HttpContext context)
    {
        return await Create(model, true);
    }

    /*public async Task<ResponseUserDTO> Create(UserDTO model, HttpContext context)
    {
        if (model.Role == UserRoles.admin)
        {
            var query = await GetUser(GetUserIdByClaims(context));
            return query.Role == UserRoles.admin ? await Create(model, true)
                : throw new TaskCanceledException("Solo un administrador puede crear otro administrador");
        }
        else
        {
            return await Create(model, false);
        }
    }*/

    public async Task<bool> Confirm(string token)
    {
        var query = await _repoModel.Query(p => p.Token == token).FirstOrDefaultAsync()
            ?? throw new TaskCanceledException("Token no valido");

        query.Token = null;
        query.IsConfirmed = true;

        var res = await _repoModel.Edit(query);
        if (!res)
            throw new TaskCanceledException("No se pudo confirmar");

        return res;
    }

    public async Task<bool> Delete(int id)
    {
        var query = await _repoModel.Query(p => p.Id == id).FirstOrDefaultAsync();
        if (query != null)
        {

            await DeleteProfileImage(query!);

            var res = await _repoModel.Delete(query);
            if (!res)
                throw new TaskCanceledException("Imposible Eliminar");

            return res;
        }
        else
        {
            throw new TaskCanceledException("No se encontro ID de usuario");
        }
    }

    public async Task<bool> Edit(UserDTO model, HttpContext context)
    {
        var query = await _repoModel.Query(p => p.Id == model.Id).FirstOrDefaultAsync();
        if (query != null)
        {
            query.FullName = model.FullName;
            query.Email = model.Email;
            var passwordIsEqual = BCrypt.Net.BCrypt.EnhancedVerify(model.Password,
                query!.Password, BCrypt.Net.HashType.SHA384);

            if (model.Password != "" && !passwordIsEqual)
            {
                query.Password = HashPassword(model.Password!);
            }
            else
            {
                query.Password = query.Password;
            }

            var profile = await GetUser(GetUserIdByClaims(context));

            if (profile.Role == UserRoles.admin)
                query.Role = model.Role;

            var res = await _repoModel.Edit(query);
            if (!res)
                throw new TaskCanceledException("No se pudo editar");

            return res;
        }
        else
        {
            throw new TaskCanceledException("Id del usuario incorrecto");
        }
    }

    // Login
    //---------------------------------------------------------------------

    public async Task<SessionDTO> Auth(LoginDTO model)
    {
        var query = await _repoModel.Query(p => p.Email == model.Email).FirstOrDefaultAsync()
           ?? throw new TaskCanceledException("Correo Electronico No Encontrado");

        if (query.IsConfirmed == false)
            throw new TaskCanceledException("Tu cuenta no ha sido confirmada");

        if (!BCrypt.Net.BCrypt.EnhancedVerify(model.Password,
                query!.Password, BCrypt.Net.HashType.SHA384))
        {
            throw new TaskCanceledException("Contraseña Incorrecta");
        }

        var jwt = _config.GetSection("JWT").GetSection("Key").Get<string>();
        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, model.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, EpochTime.GetIntDate(now).ToString(), ClaimValueTypes.Integer64),
            new Claim("Id", query.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt!));
        var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha384);
        var token = new JwtSecurityToken(
            null,
            null,
            claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: singIn
        );

        var result = new SessionDTO
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Profile = _mapper.Map<ResponseUserDTO>(query)
        };
        return result;
    }

    public async Task<ResponseUserDTO> Profile(HttpContext context)
    {
        return await GetUser(GetUserIdByClaims(context));
    }

    // User info
    //---------------------------------------------------------------------

    public async Task<ResponseUserDTO> GetUser(int id)
    {
        var query = await _repoModel.Query(p => p.Id == id).FirstOrDefaultAsync();
        if (query != null)
        {
            return _mapper.Map<ResponseUserDTO>(query);
        }
        else
        {
            throw new TaskCanceledException("No se encontro Id del usuario");
        }
    }

    private static int GetUserIdByClaims(HttpContext context)
    {
        var identity = context.User.Identity as ClaimsIdentity;
        var id = identity!.Claims.FirstOrDefault(p => p.Type == "Id")?.Value!;

        return int.Parse(id);
    }

    public async Task<List<ResponseUserDTO>> UserList(string role, string seach)
    {
        if (seach == "NA")
            seach = "";

        var query = seach == "all" ? _repoModel.Query(p => p.Role == role
            && string.Concat(
                p.FullName!.ToLower(),
                p.Email!.ToLower()
            ).Contains(seach.ToLower())
        ) ?? throw new TaskCanceledException("No se contraron usuarios") :

        _repoModel.Query(p =>
            string.Concat(
                p.FullName!.ToLower(),
                p.Email!.ToLower()
            ).Contains(seach.ToLower())
        ) ?? throw new TaskCanceledException("No se contraron usuarios");

        List<ResponseUserDTO> list = _mapper.Map<List<ResponseUserDTO>>(await query.ToListAsync());

        return list;
    }

    // Reset Password
    //---------------------------------------------------------------------

    public async Task<bool> NewPassword(ResetPasswordDTO model)
    {
        var query = await _repoModel.Query(p => p.Token == model.Token).FirstOrDefaultAsync()
            ?? throw new TaskCanceledException("Token no valido");

        query.Token = null;
        query.Password = HashPassword(model.Password!);
        var res = await _repoModel.Edit(query);
        if (!res)
            throw new TaskCanceledException("No se pudo guardar token en la base de datos");

        return res;
    }

    public async Task<bool> ResetPassword(string email)
    {
        var query = await _repoModel.Query(p => p.Email == email).FirstOrDefaultAsync()
            ?? throw new TaskCanceledException("Email no valido");

        query.Token = TokenGenerator.GenerateToken();
        var res = await _repoModel.Edit(query);
        if (!res)
            throw new TaskCanceledException("No se pudo guardar token en la base de datos");

        return res;
    }

    public async Task<string> UploadProfileImage(IFormFile upload, HttpContext context)
    {

        try
        {
            var fileExtension = Path.GetExtension(upload.FileName);
            if (!(fileExtension == ".jpg" || fileExtension == ".png"))
            {
                throw new TaskCanceledException("Solo se admiten archivos .png y .jpg");
            }

            var user = await _repoModel.Query(
                p => p.Id == GetUserIdByClaims(context)
            ).FirstOrDefaultAsync();

            var r = await DeleteProfileImage(user!);
            if (!r)
            {
                Console.WriteLine("No se pudo borrar imagen previa");
            }

            var f = upload!.FileName.Replace(fileExtension, "");

            var folder = "Images/Profile/" + f + Guid.NewGuid().ToString() + fileExtension;
            var fileName = System.IO.Path.Combine(_enviro.ContentRootPath,
                folder
            );

            user!.ImageUrl = folder;
            var res = await _repoModel.Edit(user);
            if (!res)
            {
                throw new TaskCanceledException(
                    "No se pudo guardar la url de la imagen en la base de datos"
                );
            }

            await upload.CopyToAsync(
                new System.IO.FileStream(fileName, System.IO.FileMode.Create)
            );

            return folder;
        }
        catch (Exception ex)
        {
            throw new TaskCanceledException(ex.Message);
        }
    }

    public async Task<bool> DeleteProfileImage(Usuario user)
    {
        try
        {
            if (user!.ImageUrl == "" || user!.ImageUrl == null)
            {
                return false;
            }

            var fileName = System.IO.Path.Combine(_enviro.ContentRootPath,
                user!.ImageUrl!
            );

            if (File.Exists(fileName))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                File.Delete(fileName);
            }
            else
                return false;

            user.ImageUrl = "";

            var res = await _repoModel.Edit(user);
            if (!res)
            {
                throw new TaskCanceledException(
                    "No se pudo guardar la url de la imagen en la base de datos"
                );
            }
            return res;
        }
        catch (Exception ex)
        {
            throw new TaskCanceledException(ex.Message);
        }
    }
}