using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class Registro
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo válido.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Nombre")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "El campo {0} debe tener 12 caracteres.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{12}$",
        ErrorMessage = "La contraseña debe incluir mayúscula, minúscula, número y carácter especial.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public required string ConfirmPassword { get; set; }
}
