using FrontendPublico.Modelos;
using FrontendPublico.Modelos.Vistas;
using FrontendPublico.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace FrontendPublico.Controladores;

public class CalendarioController : Controller
{
    private readonly IServicioEstadisticas _estadisticas;
    private readonly ILogger<CalendarioController> _bitacora;

    public CalendarioController(IServicioEstadisticas estadisticas, ILogger<CalendarioController> bitacora)
    {
        _estadisticas = estadisticas;
        _bitacora = bitacora;
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

        var modelo = new ModeloCalendario
        {
            FiltroEstado = estado,
            FiltroGrupo = grupo
        };

        try
        {
            modelo.Partidos = await _estadisticas.ObtenerPartidosAsync(filtroEstado, grupo);
            try
            {
                var grupos = await _estadisticas.ObtenerGruposAsync();
                modelo.GruposDisponibles = grupos.Select(g => g.Nombre).ToList();
            }
            catch (Exception excepcionGrupos)
            {
                // Los partidos pueden mostrarse aunque fallen los filtros de grupo.
                _bitacora.LogWarning(excepcionGrupos, "No se pudieron cargar los grupos para el filtro.");
            }
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "El Servicio de Estadísticas no está disponible en Calendario.");
            modelo.EstadisticasNoDisponibles = true;
        }

        return View(modelo);
    }

    public async Task<IActionResult> Detalle(long id)
    {
        try
        {
            var partido = await _estadisticas.ObtenerPartidoAsync(id);
            if (partido is null)
            {
                return NotFound();
            }

            return View(partido);
        }
        catch (Exception excepcion)
        {
            _bitacora.LogError(excepcion, "No se pudo cargar el detalle del partido {PartidoId}.", id);
            return RedirectToAction(nameof(Indice));
        }
    }
}
