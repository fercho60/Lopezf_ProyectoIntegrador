using FrontendPublico.Modelos;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class CalendarioController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;

    public CalendarioController(IServicioEstadisticas estadisticas)
    {
        _estadisticas = estadisticas;
    }

    public async Task<IActionResult> Indice(string? estado, string? grupo)
    {
        EstadoPartido? filtroEstado = estado switch
        {
            "programado" => EstadoPartido.Programado,
            "en-juego" => EstadoPartido.EnJuego,
            "finalizado" => EstadoPartido.Finalizado,
            _ => null
        };

        var partidos = await _estadisticas.ObtenerPartidosAsync(filtroEstado, grupo);
        var grupos = await _estadisticas.ObtenerGruposAsync();

        return View(new ModeloCalendario
        {
            Partidos = partidos,
            FiltroEstado = estado,
            FiltroGrupo = grupo,
            GruposDisponibles = grupos.Select(g => g.Nombre).ToList()
        });
    }

    public async Task<IActionResult> Detalle(long id)
    {
        var partido = await _estadisticas.ObtenerPartidoAsync(id);
        if (partido is null)
        {
            return NotFound();
        }
        return View(partido);
    }
}
