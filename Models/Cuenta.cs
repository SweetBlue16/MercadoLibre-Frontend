using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class ConfirmarCorreo
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo valido.")]
    [Display(Name = "Correo electronico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(64, MinimumLength = 6)]
    [Display(Name = "Codigo")]
    public string Codigo { get; set; } = string.Empty;

    public bool EmailBloqueado { get; set; }
}

public class OlvidePassword
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo valido.")]
    [Display(Name = "Correo electronico")]
    public string Email { get; set; } = string.Empty;
}

public class RestablecerPassword
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo valido.")]
    [Display(Name = "Correo electronico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(128, MinimumLength = 6)]
    [Display(Name = "Codigo")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MinLength(12, ErrorMessage = "El campo {0} debe tener un minimo de {1} caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12,128}$",
        ErrorMessage = "La contrasena debe incluir mayuscula, minuscula, numero y caracter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contrasena")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Compare(nameof(Password), ErrorMessage = "Las contrasenas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contrasena")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class CambioPassword
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(64, MinimumLength = 6)]
    [Display(Name = "Codigo")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contrasena actual")]
    public string PasswordActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [MinLength(12, ErrorMessage = "El campo {0} debe tener un minimo de {1} caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12,128}$",
        ErrorMessage = "La contrasena debe incluir mayuscula, minuscula, numero y caracter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contrasena")]
    public string PasswordNueva { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Compare(nameof(PasswordNueva), ErrorMessage = "Las contrasenas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contrasena")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class PerfilViewModel
{
    public AuthUser Usuario { get; set; } = new()
    {
        Email = string.Empty,
        Nombre = string.Empty,
        Rol = string.Empty,
        Jwt = string.Empty,
    };

    public string TiempoRestanteSesion { get; set; } = string.Empty;
    public CambioPassword CambioPassword { get; set; } = new();
    public bool CodigoCambioPasswordSolicitado { get; set; }
    public bool MostrarRol => string.Equals(Usuario.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);
}
