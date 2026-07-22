using FrontendPublico.Extensiones;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

[Authorize]
public class PrediccionesController : Controller
{
    private readonly IServicioMonedas _monedas;
    private readonly IServicioEstadisticas _estadisticas;

    public PrediccionesController(IServicioMonedas monedas, IServicioEstadisticas estadisticas)
    {
        _monedas = monedas;
        _estadisticas = estadisticas;
    }

    public async Task<IActionResult> Indice()
    {
        var predicciones = await _monedas.ObtenerPrediccionesAsync(User.ObtenerUsuarioId());
        return View(predicciones);
    }

    [HttpGet]
    public async Task<IActionResult> Crear(long partidoId)
    {
        var partido = await _estadisticas.ObtenerPartidoAsync(partidoId);
        if (partido is null)
        {
            return NotFound();
        }

        if (!partido.AdmitePredicciones)
        {
            TempData["Error"] = "Las predicciones de este partido están cerradas: el encuentro ya inició o finalizó.";
            return RedirectToAction("Detalle", "Calendario", new { id = partidoId });
        }

        return View(new ModeloCrearPrediccion
        {
            PartidoId = partido.Id,
            Partido = partido,
            SaldoDisponible = await _monedas.ObtenerSaldoAsync(User.ObtenerUsuarioId())
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(ModeloCrearPrediccion modelo)
    {
        var usuarioId = User.ObtenerUsuarioId();

        async Task<IActionResult> ReintentarAsync()
        {
            modelo.Partido = await _estadisticas.ObtenerPartidoAsync(modelo.PartidoId);
            modelo.SaldoDisponible = await _monedas.ObtenerSaldoAsync(usuarioId);
            return View(modelo);
        }

        if (!ModelState.IsValid)
        {
            return await ReintentarAsync();
        }

        try
        {
            var prediccion = await _monedas.CrearPrediccionAsync(
                usuarioId, modelo.PartidoId, modelo.Pronostico!.Value, modelo.Monto);
            TempData["Exito"] =
                $"Predicción registrada: {prediccion.DescripcionPartido}. " +
                $"Premio potencial: {prediccion.PremioPotencial:0.##} UTNGolCoin.";
            return RedirectToAction(nameof(Indice));
        }
        catch (ExcepcionReglaNegocio excepcion)
        {
            ModelState.AddModelError(string.Empty, excepcion.Message);
            return await ReintentarAsync();
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "No se pudo registrar la predicción: el servicio de monedas no respondió. Revisa que UTNGolCoin esté arriba en la red.");
            return await ReintentarSeguroAsync();
        }
        catch (TaskCanceledException)
        {
            // HttpClient.Timeout (15s) → TaskCanceledException, no HttpRequestException.
            ModelState.AddModelError(
                string.Empty,
                "Tiempo de espera agotado al contactar UTNGolCoin (15s). La API de Mayra no responde en http://…:5000 — firewall, IP o proceso caído.");
            return await ReintentarSeguroAsync();
        }

        async Task<IActionResult> ReintentarSeguroAsync()
        {
            try
            {
                return await ReintentarAsync();
            }
            catch
            {
                modelo.Partido ??= await _estadisticas.ObtenerPartidoAsync(modelo.PartidoId);
                return View(modelo);
            }
        }
    }
}
