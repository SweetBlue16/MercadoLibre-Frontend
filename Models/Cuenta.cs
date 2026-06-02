using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class ConfirmarCorreo
{
    [Required(ErrorMessage = "No hay una verificación de correo pendiente. Regístrate nuevamente.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo válido.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el código de verificación.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código de verificación no es válido.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código de verificación no es válido.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    public bool EmailBloqueado { get; set; }
}

public class OlvidePassword
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo válido.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;
}

public class RestablecerPassword
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo válido.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el código de verificación.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código de verificación no es válido.")]
    [Display(Name = "Código")]
    public string Token { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "El campo {0} debe tener 12 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12}$",
        ErrorMessage = "La contraseña debe incluir mayúscula, minúscula, número y carácter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class CambioPassword
{
    [Required(ErrorMessage = "Ingresa el código de verificación.")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "El código de verificación no es válido.")]
    [Display(Name = "Código")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string PasswordActual { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "El campo {0} debe tener 12 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12}$",
        ErrorMessage = "La contraseña debe incluir mayúscula, minúscula, número y carácter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string PasswordNueva { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Compare(nameof(PasswordNueva), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nueva contraseña")]
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

    public CambioPassword CambioPassword { get; set; } = new();
    public bool CodigoCambioPasswordSolicitado { get; set; }
    public bool MostrarRol => string.Equals(Usuario.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);
}
