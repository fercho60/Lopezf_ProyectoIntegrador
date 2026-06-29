using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class EstadisticasController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;

    public EstadisticasController(IServicioEstadisticas estadisticas)
    {
        _estadisticas = estadisticas;
    }

    public async Task<IActionResult> Indice()
    {
        var estadisticas = await _estadisticas.ObtenerEstadisticasSeleccionesAsync();
        return View(estadisticas);
    }
}
