using FrontendEstadisticas.Modelos;
using FrontendEstadisticas.Modelos.Vistas;
using FrontendEstadisticas.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendEstadisticas.Controladores;

public class InicioController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;
    private readonly ILogger<InicioController> _bitacora;
    private readonly IConfiguration _configuracion;

    public InicioController(
        IServicioEstadisticas estadisticas,
        ILogger<InicioController> bitacora,
        IConfiguration configuracion)
    {
        _estadisticas = estadisticas;
        _bitacora = bitacora;
        _configuracion = configuracion;
    }

    public async Task<IActionResult> Indice()
    {
        var modelo = new ModeloInicio
        {
            UrlApuestas = _configuracion["Frontends:ApuestasUrl"] ?? "http://localhost:5081"
        };

        try
        {
            modelo.PartidosEnJuego = await _estadisticas.ObtenerPartidosAsync(EstadoPartido.EnJuego);
            var programados = await _estadisticas.ObtenerPartidosAsync(EstadoPartido.Programado);
            modelo.ProximosPartidos = programados.Take(4).ToList();
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "El Servicio de Estadísticas no está disponible.");
            modelo.EstadisticasNoDisponibles = true;
        }

        return View(modelo);
    }
}
