using System.Globalization;
using FrontendEstadisticas.Servicios;
using FrontendEstadisticas.Servicios.Http;
using FrontendEstadisticas.Servicios.Simulados;
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

var usarSimulado = constructor.Configuration.GetValue("Servicios:Estadisticas:UsarSimulado", false);
var direccionEstadisticas = constructor.Configuration["Servicios:Estadisticas:DireccionBase"]
    ?? "http://localhost:8080/demo/api/v1/";

if (usarSimulado)
{
    constructor.Services.AddSingleton<AlmacenDatosSimulados>();
    constructor.Services.AddSingleton<IServicioEstadisticas, ServicioEstadisticasSimulado>();
}
else
{
    constructor.Services.AddHttpClient<IServicioEstadisticas, ServicioEstadisticasHttp>(cliente =>
    {
        cliente.BaseAddress = new Uri(direccionEstadisticas.EndsWith('/') ? direccionEstadisticas : direccionEstadisticas + "/");
        cliente.Timeout = TimeSpan.FromSeconds(15);
    });
    // Respaldo simulado si se necesita inyectar Almacen en otros sitios
    constructor.Services.AddSingleton<AlmacenDatosSimulados>();
}

var aplicacion = constructor.Build();

aplicacion.UseExceptionHandler("/Errores/Error");
aplicacion.UseStatusCodePagesWithReExecute("/Errores/Codigo/{0}");

aplicacion.UseStaticFiles();
aplicacion.UseRouting();

aplicacion.MapControllerRoute(
    name: "predeterminada",
    pattern: "{controller=Inicio}/{action=Indice}/{id?}");

aplicacion.Run();
