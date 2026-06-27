using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios;

/// <summary>
/// Puerto hacia el Servicio de Estadísticas (Jakarta EE + PostgreSQL).
/// Contrato: docs/contratos/estadisticas-openapi.yaml
/// </summary>
public interface IServicioEstadisticas
{
    Task<List<Grupo>> ObtenerGruposAsync();
    Task<List<Partido>> ObtenerPartidosAsync(EstadoPartido? estado = null, string? grupo = null);
    Task<Partido?> ObtenerPartidoAsync(long partidoId);
    Task<List<EstadisticaSeleccion>> ObtenerEstadisticasSeleccionesAsync();
}
