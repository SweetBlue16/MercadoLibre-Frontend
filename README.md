# Mercado Libre Frontend

Frontend ASP.NET Core MVC para Mercado Libre Seguro. Consume el backend en `UrlWebAPI`, usa cookies HttpOnly y autorizacion por roles.

## Comandos

```powershell
dotnet restore
dotnet build
dotnet run --urls http://localhost:8080
```

## Configuracion

`appsettings.json` y `appsettings.Development.json` deben definir:

```json
{
  "UrlWebAPI": "http://localhost:3000"
}
```

La clave antigua `URLWebAPI` se acepta como fallback en `Program.cs`, pero la clave unificada es `UrlWebAPI`.

## Funciones

- Login y registro publico.
- Productos/categorias/usuarios/archivos/bitacora para administrador.
- Catalogo de compra, carrito y confirmacion de pedido para usuario.
- Pedidos recibidos para administrador.
- Imagenes de producto por `ArchivoId` usando `/api/archivos/{ArchivoId}` y placeholder local `/images/temp.png`.
