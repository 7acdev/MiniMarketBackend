using System;
using System.Collections.Generic;

namespace Minimarket.Model;

public partial class Usuario
{
    public int Id { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public string? Password { get; set; }

    public string? ImageUrl { get; set; }

    public string? Token { get; set; }

    public bool? IsConfirmed { get; set; }

    public DateTime? RegisterDate { get; set; }
}
