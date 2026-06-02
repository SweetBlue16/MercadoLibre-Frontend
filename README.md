# Mercado Libre Seguro - Frontend

Frontend ASP.NET Core MVC para consumir el backend configurado en `UrlWebAPI`. Usa cookies HttpOnly, autorización por roles, antiforgery global, cabeceras de seguridad centralizadas y servicios `HttpClient`.

## Configuración

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
dotnet run --urls http://localhost:5000
```

## Flujos

- Login con cookie HttpOnly; el JWT queda en claims de la cookie y no se renderiza en HTML.
- Registro público muestra la política de password y siempre solicita rol `Usuario` al backend.
- Confirmación de correo en `/Auth/ConfirmarCorreo`.
- Recuperación en `/Auth/OlvidePassword` y `/Auth/RestablecerPassword`.
- Perfil en `/Perfil` muestra email, nombre, rol, correo confirmado y cambio de contraseña.
- Usuario: Comprar, Carrito, Mis pedidos y Perfil.
- Administrador: Productos, Categorías, Archivos, Usuarios, Pedidos y Bitácora.

## Imágenes

- La construcción de URLs está centralizada en `Helpers/ImagenUrlHelper.cs`.
- El listado de productos consume `GET /api/productos`; el backend devuelve `ImagenUrl` como ruta relativa, por ejemplo `/api/archivos/5`.
- Razor combina `ImagenUrl` con `UrlWebAPI` desde configuración y renderiza directamente `<img src="...">`.
- Si `ImagenUrl` no viene pero `ArchivoId` existe, el helper conserva compatibilidad construyendo `/api/archivos/{ArchivoId}` con `UrlWebAPI`.
- Si no hay `ArchivoId`, se usa placeholder local `/images/temp.png`.
- `wwwroot/images/imagenes-productos` no es fuente principal para productos con `ArchivoId`; queda como recurso estático auxiliar/fallback.
- No se renderizan rutas físicas, `uploads`, `wwwroot`, `C:\...` ni placeholders externos.

## Seguridad MVC

- `SecurityHeadersMiddleware` agrega CSP, anti-clickjacking, `nosniff`, `Referrer-Policy`, `Permissions-Policy` y `Cross-Origin-Opener-Policy`.
- `Program.cs` desactiva `Server: Kestrel` con `options.AddServerHeader = false`.
- En producción se usa `UseHsts()` y `UseHttpsRedirection()`.
- `NoStoreCacheMiddleware` aplica `Cache-Control: no-store, no-cache, must-revalidate` a páginas autenticadas o sensibles sin afectar assets estáticos.
- Cookies HttpOnly, SameSite Lax y Secure en producción.
- `AutoValidateAntiforgeryTokenAttribute` global para formularios POST.
- Manejo centralizado de errores en `Services/Errors`.
- No se guarda JWT en localStorage ni se expone en vistas.

La CSP permite recursos locales, imágenes `data:`/`blob:` y el origen de API configurado. En desarrollo también permite orígenes locales necesarios. `style-src 'unsafe-inline'` se mantiene temporalmente porque varias vistas Razor y componentes Bootstrap usan estilos inline; `script-src` no permite scripts inline.

## Confirmación de correo

Después del registro, `/Auth/ConfirmarCorreo` conserva el email en `TempData` protegida, muestra el campo como solo lectura y solo permite capturar el código. Si se entra manualmente sin contexto protegido, se muestra una vista controlada con mensaje claro.

## Alcance OWASP ZAP

Para futuros escaneos, crear un Context de ZAP que incluya únicamente la aplicación:

- Frontend MVC local o Azure.
- Backend API local o Azure.

Ejemplos locales:

- `^https?://localhost:5000/.*`
- `^https?://localhost:3000/.*`

Excluir tráfico externo del navegador que no pertenece al alcance:

- `.*googleapis\.com.*`
- `.*google\.com.*`
- `.*gvt2\.com.*`
- `.*clients\.google\.com.*`
- `.*accounts\.google\.com.*`

La alerta de ZAP "Petición de Autenticación Identificada" en el backend es informativa. Debe revisarse que login no devuelva passwords, que los errores estén controlados y que el rate limiting siga activo, pero no requiere un cambio de código por sí sola.
