using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class Login
{
    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [EmailAddress(ErrorMessage = "El campo {0} no es correo válido.")]
    [StringLength(40, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Correo electrónico")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(12, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public required string Password { get; set; }
}
