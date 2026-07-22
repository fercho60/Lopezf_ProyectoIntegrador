using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class PosicionesController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;
    private readonly ILogger<PosicionesController> _bitacora;

    public PosicionesController(IServicioEstadisticas estadisticas, ILogger<PosicionesController> bitacora)
    {
        _estadisticas = estadisticas;
        _bitacora = bitacora;
    }

    public async Task<IActionResult> Indice()
    {
        try
        {
            return View(await _estadisticas.ObtenerGruposAsync());
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "El Servicio de Estadísticas no está disponible en Posiciones.");
            ViewBag.EstadisticasNoDisponibles = true;
            return View(new List<FrontendPublico.Modelos.Grupo>());
        }
    }
}
