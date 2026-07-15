using System.Globalization;
using FrontendPublico.Servicios;
using FrontendPublico.Servicios.Http;
using FrontendPublico.Servicios.Simulados;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;

var cultura = new CultureInfo("es-EC");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllersWithViews();

constructor.Services.Configure<RazorViewEngineOptions>(opciones =>
{
    opciones.ViewLocationFormats.Clear();
    opciones.ViewLocationFormats.Add("/Vistas/{1}/{0}" + RazorViewEngine.ViewExtension);
    opciones.ViewLocationFormats.Add("/Vistas/Compartido/{0}" + RazorViewEngine.ViewExtension);
});

constructor.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opciones =>
    {
        opciones.LoginPath = "/Cuenta/Iniciar";
        opciones.AccessDeniedPath = "/Cuenta/Iniciar";
        opciones.Cookie.Name = "UTNGolMundial.Sesion";
        opciones.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

var usarEstadisticasSimulado = constructor.Configuration.GetValue("Servicios:Estadisticas:UsarSimulado", false);
var usarMonedasSimulado = constructor.Configuration.GetValue("Servicios:UTNGolCoin:UsarSimulado", false);
var direccionEstadisticas = constructor.Configuration["Servicios:Estadisticas:DireccionBase"]
    ?? "http://localhost:8080/demo/api/v1/";
var direccionMonedas = constructor.Configuration["Servicios:UTNGolCoin:DireccionBase"]
    ?? "http://localhost:5000/api/";

constructor.Services.AddSingleton<AlmacenDatosSimulados>();

if (usarEstadisticasSimulado)
{
    constructor.Services.AddSingleton<IServicioEstadisticas, ServicioEstadisticasSimulado>();
    constructor.Services.AddSingleton<IServicioAutenticacion, ServicioAutenticacionSimulado>();
}
else
{
    constructor.Services.AddHttpClient<IServicioEstadisticas, ServicioEstadisticasHttp>(cliente =>
    {
        cliente.BaseAddress = new Uri(direccionEstadisticas.EndsWith('/') ? direccionEstadisticas : direccionEstadisticas + "/");
        cliente.Timeout = TimeSpan.FromSeconds(15);
    });
    constructor.Services.AddHttpClient<IServicioAutenticacion, ServicioAutenticacionHttp>(cliente =>
    {
        cliente.BaseAddress = new Uri(direccionEstadisticas.EndsWith('/') ? direccionEstadisticas : direccionEstadisticas + "/");
        cliente.Timeout = TimeSpan.FromSeconds(15);
    });
}

if (usarMonedasSimulado)
{
    constructor.Services.AddSingleton<IServicioMonedas, ServicioMonedasSimulado>();
}
else
{
    constructor.Services.AddHttpClient<IServicioMonedas, ServicioMonedasHttp>(cliente =>
    {
        cliente.BaseAddress = new Uri(direccionMonedas.EndsWith('/') ? direccionMonedas : direccionMonedas + "/");
        cliente.Timeout = TimeSpan.FromSeconds(15);
    });
}

var aplicacion = constructor.Build();

aplicacion.UseExceptionHandler("/Errores/Error");
aplicacion.UseStatusCodePagesWithReExecute("/Errores/Codigo/{0}");

aplicacion.UseStaticFiles();
aplicacion.UseRouting();
aplicacion.UseAuthentication();
aplicacion.UseAuthorization();

aplicacion.MapControllerRoute(
    name: "predeterminada",
    pattern: "{controller=Inicio}/{action=Indice}/{id?}");

aplicacion.Run();
