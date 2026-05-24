# Mercado Libre Seguro - Frontend

Frontend ASP.NET Core MVC para consumir el backend en `UrlWebAPI`. Usa cookies HttpOnly, autorizacion por roles, antiforgery global y servicios `HttpClient`.

## Configuracion

`appsettings.json` debe definir:

```json
{
  "UrlWebAPI": "http://localhost:3000"
}
```

La clave antigua `URLWebAPI` se conserva como fallback temporal, pero el uso principal es `UrlWebAPI`.

## Comandos

```powershell
dotnet restore
dotnet build
dotnet run --urls http://localhost:8080
```

## Flujos

- Login con cookie HttpOnly; el JWT queda en claims de la cookie y no se renderiza en HTML.
- Registro publico muestra la politica de password y siempre solicita rol `Usuario` al backend.
- Confirmacion de correo en `/Auth/ConfirmarCorreo`.
- Recuperacion en `/Auth/OlvidePassword` y `/Auth/RestablecerPassword`.
- Perfil en `/Perfil` muestra email, nombre, rol, correo confirmado y tiempo restante como minutos/segundos; incluye cambio de contraseña.
- Usuario: Comprar, Carrito, Mis pedidos y Perfil.
- Administrador: Productos, Categorias, Archivos, Usuarios, Pedidos y Bitacora.
- Admin puede actualizar estado de pedido; usuario ve el estado actualizado en Mis pedidos.

## Imagenes

- La construccion de URLs esta centralizada en `Helpers/ImagenUrlHelper.cs`.
- El listado de productos consume `GET /api/productos`; el backend devuelve `ImagenUrl` como ruta relativa, por ejemplo `/api/archivos/5`.
- Razor combina `ImagenUrl` con `UrlWebAPI` desde configuracion y renderiza directamente `<img src="...">`.
- Si `ImagenUrl` no viene pero `ArchivoId` existe, el helper conserva compatibilidad construyendo `/api/archivos/{ArchivoId}` con `UrlWebAPI`.
- Si no hay `ArchivoId`, se usa placeholder local `/images/temp.png`.
- `wwwroot/images/imagenes-productos` no es fuente principal para productos con `ArchivoId`; solo queda como recurso estatico auxiliar/fallback.
- No se renderizan rutas fisicas, `uploads`, `wwwroot`, `C:\...` ni placeholders externos.
- Es normal que el navegador solicite cada imagen como recurso. Lo que se evita es hacer llamadas programaticas adicionales por producto para obtener metadata de archivos.

## Seguridad MVC

- Cookies HttpOnly, SameSite Lax y Secure en produccion.
- `AutoValidateAntiforgeryTokenAttribute` global para formularios POST.
- Validacion de modelos en cliente y servidor.
- Manejo centralizado de errores en `Services/Errors`: el frontend lee `code` del API, lo traduce con `MessageCatalog` y evita mostrar errores tecnicos como `Bad Request`, stack traces o `Response status code does not indicate success`.
- Si el API esta apagada, timeout o conexion rechazada, se muestra: `El servidor no está disponible en este momento. Inténtalo más tarde.`
- Manejo de 401 redirigiendo a salida/login.
- No se guarda JWT en localStorage ni se expone en vistas.

## Confirmacion de correo

Despues del registro, `/Auth/ConfirmarCorreo` conserva el email en `TempData` protegida, muestra el campo como solo lectura y solo permite capturar el codigo. Si se entra manualmente sin contexto protegido, el formulario permite escribir email y codigo.
