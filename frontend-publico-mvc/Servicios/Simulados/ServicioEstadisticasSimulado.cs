using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios.Simulados;

/// <summary>
/// Implementación simulada del Servicio de Estadísticas. Se sustituirá por un cliente
/// HTTP contra docs/contratos/estadisticas-openapi.yaml cuando el backend exista.
/// </summary>
public class ServicioEstadisticasSimulado : IServicioEstadisticas
{
    private readonly AlmacenDatosSimulados _almacen;

    public ServicioEstadisticasSimulado(AlmacenDatosSimulados almacen)
    {
        _almacen = almacen;
    }

    public Task<List<Grupo>> ObtenerGruposAsync()
    {
        lock (_almacen.Candado)
        {
            return Task.FromResult(_almacen.Grupos.ToList());
        }
    }

    public Task<List<Partido>> ObtenerPartidosAsync(EstadoPartido? estado = null, string? grupo = null)
    {
        lock (_almacen.Candado)
        {
            var consulta = _almacen.Partidos.AsEnumerable();
            if (estado.HasValue)
                consulta = consulta.Where(p => p.Estado == estado.Value);
            if (!string.IsNullOrWhiteSpace(grupo))
                consulta = consulta.Where(p => p.Grupo == grupo);
            return Task.FromResult(consulta.OrderBy(p => p.FechaHora).ToList());
        }
    }

    public Task<Partido?> ObtenerPartidoAsync(long partidoId)
    {
        lock (_almacen.Candado)
        {
            return Task.FromResult(_almacen.Partidos.FirstOrDefault(p => p.Id == partidoId));
        }
    }

    public Task<List<EstadisticaSeleccion>> ObtenerEstadisticasSeleccionesAsync()
    {
        lock (_almacen.Candado)
        {
            var estadisticas = _almacen.Grupos
                .SelectMany(g => g.Posiciones)
                .Select(p => new EstadisticaSeleccion
                {
                    Seleccion = p.Seleccion,
                    Jugados = p.Jugados,
                    Ganados = p.Ganados,
                    Empatados = p.Empatados,
                    Perdidos = p.Perdidos,
                    GolesFavor = p.GolesFavor,
                    GolesContra = p.GolesContra
                })
                .OrderByDescending(e => e.GolesFavor)
                .ToList();
            return Task.FromResult(estadisticas);
        }
    }
}
