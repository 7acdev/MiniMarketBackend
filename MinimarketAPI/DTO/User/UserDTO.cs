using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Minimarket.DTO;

public class UserDTO
{
    public int? Id { get; set; }

    private string? _fullName;

    [Required(ErrorMessage = "Ingresa tu nombre")]
    public string? FullName
    {
        get => _fullName;
        set => _fullName = value!.Trim();
    }

    private string? _email;

    [Required(ErrorMessage = "Ingresa tú correo electronico")]
    public string? Email
    {
        get => _email;
        set => _email = value!.Trim().ToLower();
    }

    private string? _password;
    public string? Password
    {
        get => _password;
        set => _password = value!.Trim();
    }

    private string? _role;
    public string? Role
    {
        get => _role;
        set => _role = value!.Trim();
    }

    public string? ImageUrl { get; set; }
}