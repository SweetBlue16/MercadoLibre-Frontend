using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class Producto
{
    public const int TextMaxLength = 40;
    public const int DescripcionMaxLength = 300;

    [Display(Name = "Id")]
    public int? ProductoId { get; set; }

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(TextMaxLength, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [Display(Name = "Título")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [StringLength(DescripcionMaxLength, ErrorMessage = "El campo {0} no debe exceder {1} caracteres.")]
    [DataType(DataType.MultilineText)]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; } = "Sin descripción";

    [Required(ErrorMessage = "El campo {0} es obligatorio.")]
    [DataType(DataType.Currency)]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "El valor del campo debe ser un precio válido.")]
    [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
    [Display(Name = "Precio")]
    public decimal Precio { get; set; }

    [Display(Name = "Portada")]
    public int? ArchivoId { get; set; }

    public string? ImagenUrl { get; set; }

    [Display(Name = "Eliminable")]
    public bool Protegida { get; set; } = false;

    public ICollection<Categoria>? Categorias { get; set; }
}
