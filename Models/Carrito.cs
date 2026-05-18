using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class CarritoItem
{
    public int CarritoId { get; set; }
    public int ProductoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }

    public int? ArchivoId { get; set; }

    [Range(1, 99)]
    public int Cantidad { get; set; }

    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }
}

public class CarritoResumen
{
    public List<CarritoItem> Items { get; set; } = [];

    [DataType(DataType.Currency)]
    public decimal Total { get; set; }
}
