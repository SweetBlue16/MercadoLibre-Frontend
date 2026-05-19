using System.ComponentModel.DataAnnotations;

namespace frontendnet.Models;

public class PedidoUsuario
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class DetallePedido
{
    public int DetallePedidoId { get; set; }
    public int ProductoId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int Cantidad { get; set; }

    [DataType(DataType.Currency)]
    public decimal Precio { get; set; }

    [DataType(DataType.Currency)]
    public decimal Subtotal { get; set; }

    public Producto? Producto { get; set; }
}

public class Pedido
{
    public int PedidoId { get; set; }
    public DateTime Fecha { get; set; }

    [DataType(DataType.Currency)]
    public decimal Total { get; set; }

    public string Estado { get; set; } = "Recibido";

    public PedidoUsuario? Usuario { get; set; }
    public List<DetallePedido> Detalles { get; set; } = [];
}

public class ActualizarEstadoPedido
{
    [Required]
    public string Estado { get; set; } = "Recibido";
}
