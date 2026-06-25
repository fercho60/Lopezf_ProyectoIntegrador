using System.Globalization;
using FrontendPublico.Servicios;
using FrontendPublico.Servicios.Simulados;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;

var cultura = new CultureInfo("es-EC");
CultureInfo.DefaultThreadCurrentCulture = cultura;
CultureInfo.DefaultThreadCurrentUICulture = cultura;

var constructor = WebApplication.CreateBuilder(args);

constructor.Services.AddControllersWithViews();

// Las vistas viven en /Vistas (en español) en lugar de /Views
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

// Almacén compartido de datos simulados y servicios de la fase UI-primero.
// Cuando los backends estén listos, estas líneas se reemplazan por clientes HTTP
// que implementen las mismas interfaces (ver docs/contratos/).
constructor.Services.AddSingleton<AlmacenDatosSimulados>();
constructor.Services.AddSingleton<IServicioEstadisticas, ServicioEstadisticasSimulado>();
constructor.Services.AddSingleton<IServicioAutenticacion, ServicioAutenticacionSimulado>();
constructor.Services.AddSingleton<IServicioMonedas, ServicioMonedasSimulado>();

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
