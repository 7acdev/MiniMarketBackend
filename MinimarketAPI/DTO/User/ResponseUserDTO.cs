namespace Minimarket.DTO;

public class ResponseUserDTO
{
    public int? Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public string? ImageUrl { get; set; }

    //public string? Msg { get; set; } // Usar si se evita el try-catch.
}