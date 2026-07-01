using System.Security.Claims;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class CuentaController : Controller
{
    private readonly IServicioAutenticacion _autenticacion;
    private readonly IServicioMonedas _monedas;

    public CuentaController(IServicioAutenticacion autenticacion, IServicioMonedas monedas)
    {
        _autenticacion = autenticacion;
        _monedas = monedas;
    }

    [HttpGet]
    public IActionResult Registro() => View(new ModeloRegistro());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Registro(ModeloRegistro modelo)
    {
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        try
        {
            var usuario = await _autenticacion.RegistrarAsync(modelo.Nombre, modelo.Correo, modelo.Contrasena);
            await CrearSesionAsync(usuario.Id, usuario.Nombre, usuario.Rol);
            TempData["Exito"] = "¡Bienvenido! Recibiste 10 UTNGolCoin de regalo para comenzar a predecir.";
            return RedirectToAction("Indice", "Inicio");
        }
        catch (ExcepcionReglaNegocio excepcion)
        {
            ModelState.AddModelError(nameof(modelo.Correo), excepcion.Message);
            return View(modelo);
        }
    }

    [HttpGet]
    public IActionResult Iniciar(string? returnUrl = null)
    {
        ViewData["UrlRetorno"] = returnUrl;
        return View(new ModeloInicioSesion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Iniciar(ModeloInicioSesion modelo, string? returnUrl = null)
    {
        ViewData["UrlRetorno"] = returnUrl;
        if (!ModelState.IsValid)
        {
            return View(modelo);
        }

        var usuario = await _autenticacion.IniciarSesionAsync(modelo.Correo, modelo.Contrasena);
        if (usuario is null)
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(modelo);
        }

        await CrearSesionAsync(usuario.Id, usuario.Nombre, usuario.Rol);

        // RF20: bono diario anti-bancarrota al iniciar sesión con saldo cero
        var bonoOtorgado = await _monedas.ReclamarBonoDiarioAsync(usuario.Id);
        if (bonoOtorgado)
        {
            TempData["Alerta"] = "Tu saldo estaba en cero: se acreditó 1 UTNGolCoin de bono diario para que sigas participando.";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Indice", "Inicio");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cerrar()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Exito"] = "Sesión cerrada correctamente. ¡Vuelve pronto!";
        return RedirectToAction("Indice", "Inicio");
    }

    private async Task CrearSesionAsync(long usuarioId, string nombre, string rol)
    {
        var reclamos = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuarioId.ToString()),
            new(ClaimTypes.Name, nombre),
            new(ClaimTypes.Role, rol)
        };
        var identidad = new ClaimsIdentity(reclamos, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identidad));
    }
}
