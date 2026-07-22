using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class EstadisticasController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;
    private readonly ILogger<EstadisticasController> _bitacora;

    public EstadisticasController(IServicioEstadisticas estadisticas, ILogger<EstadisticasController> bitacora)
    {
        _estadisticas = estadisticas;
        _bitacora = bitacora;
    }

    public async Task<IActionResult> Indice()
    {
        try
        {
            return View(await _estadisticas.ObtenerEstadisticasSeleccionesAsync());
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "El Servicio de Estadísticas no está disponible.");
            ViewBag.EstadisticasNoDisponibles = true;
            return View(new List<FrontendPublico.Modelos.EstadisticaSeleccion>());
        }
    }
}
