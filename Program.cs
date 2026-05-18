using frontendnet.Middlewares;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Agregamos los servicios
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

// Soporte para consultar el API
var UrlWebAPI = builder.Configuration["UrlWebAPI"] ?? builder.Configuration["URLWebAPI"];

if (string.IsNullOrWhiteSpace(UrlWebAPI))
{
    throw new InvalidOperationException("No se encontró la configuración UrlWebAPI en appsettings.json.");
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<EnviaBearerDelegatingHandler>();
builder.Services.AddTransient<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<AuthClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
});

builder.Services.AddHttpClient<CategoriasClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<UsuariosClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<RolesClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<ProductosClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<PerfilClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<ArchivosClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<BitacoraClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<CarritoClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

builder.Services.AddHttpClient<PedidosClientService>(httpClient =>
{
    httpClient.BaseAddress = new Uri(UrlWebAPI);
})
.AddHttpMessageHandler<EnviaBearerDelegatingHandler>()
.AddHttpMessageHandler<RefrescaTokenDelegatingHandler>();

// Soporte para Cookie Auth
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".frontendnet";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.LoginPath = "/Auth";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

var app = builder.Build();

// Configuración del pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
