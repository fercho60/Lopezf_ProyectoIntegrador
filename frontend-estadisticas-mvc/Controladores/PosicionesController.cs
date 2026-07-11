using FrontendEstadisticas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendEstadisticas.Controladores;

public class PosicionesController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;

    public PosicionesController(IServicioEstadisticas estadisticas)
    {
        _estadisticas = estadisticas;
    }

    public async Task<IActionResult> Indice()
    {
        var grupos = await _estadisticas.ObtenerGruposAsync();
        return View(grupos);
    }
}
