using System.ComponentModel.DataAnnotations;

namespace Minimarket.DTO;

public class LoginDTO
{
    private string? _email;

    [Required(ErrorMessage = "Please insert your email")]
    public string? Email
    {
        get => _email;
        set => _email = value!.Trim();
    }

    private string? _password;

    [Required(ErrorMessage = "Please insert your password")]
    public string? Password
    {
        get => _password;
        set => _password = value!.Trim();
    }
}