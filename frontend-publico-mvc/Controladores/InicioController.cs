using FrontendPublico.Modelos;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class InicioController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;
    private readonly IServicioMonedas _monedas;
    private readonly ILogger<InicioController> _bitacora;

    public InicioController(IServicioEstadisticas estadisticas, IServicioMonedas monedas, ILogger<InicioController> bitacora)
    {
        _estadisticas = estadisticas;
        _monedas = monedas;
        _bitacora = bitacora;
    }

    public async Task<IActionResult> Indice()
    {
        var modelo = new ModeloInicio();

        // Degradación controlada (RNF05): si un servicio falla, la página sigue funcionando.
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

        try
        {
            var ranking = await _monedas.ObtenerRankingAsync();
            modelo.TopRanking = ranking.Take(5).ToList();
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "El Servicio UTNGolCoin no está disponible.");
            modelo.MonedasNoDisponibles = true;
        }

        return View(modelo);
    }
}
