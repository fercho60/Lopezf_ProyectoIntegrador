using System.Text.Json;
using FrontendEstadisticas.Modelos;
using FrontendEstadisticas.Servicios;
using Microsoft.Extensions.Logging;

namespace FrontendEstadisticas.Servicios.Http;

/// <summary>
/// Cliente HTTP hacia el Servicio de Estadísticas (Guacales).
/// Base: http://localhost:8080/demo/api/v1/
/// </summary>
public class ServicioEstadisticasHttp : IServicioEstadisticas
{
    private readonly HttpClient _http;
    private readonly ILogger<ServicioEstadisticasHttp> _bitacora;

    public ServicioEstadisticasHttp(HttpClient http, ILogger<ServicioEstadisticasHttp> bitacora)
    {
        _http = http;
        _bitacora = bitacora;

        if (_http.BaseAddress is null)
        {
            _http.BaseAddress = new Uri("http://localhost:8080/demo/api/v1/");
        }
    }

    public async Task<List<Grupo>> ObtenerGruposAsync()
    {
        var json = await ObtenerJsonExitoAsync("grupos", "listar grupos");
        return MapearLista(json, MapeoEstadisticasJson.AGrupo);
    }

    public async Task<List<Partido>> ObtenerPartidosAsync(EstadoPartido? estado = null, string? grupo = null)
    {
        // Guacales: GET /partidos sin filtro funciona; ?estado=PROGRAMADO puede devolver 500
        // (bug Hibernate lazy de Grupo). Pedimos todo y filtramos aquí.
        var json = await ObtenerJsonExitoAsync("partidos", "listar partidos");
        var partidos = MapearLista(json, MapeoEstadisticasJson.APartido);
        RegistrarMuestraBanderas(partidos);

        if (estado.HasValue)
        {
            partidos = partidos.Where(p => p.Estado == estado.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(grupo))
        {
            partidos = partidos
                .Where(p => string.Equals(p.Grupo, grupo, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return partidos;
    }

    public async Task<Partido?> ObtenerPartidoAsync(long partidoId)
    {
        using var respuesta = await _http.GetAsync($"partidos/{partidoId}");

        if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        var contenido = await respuesta.Content.ReadAsStringAsync();
        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al obtener partido {PartidoId}: {Contenido}",
                (int)respuesta.StatusCode,
                partidoId,
                contenido);
            throw new HttpRequestException($"No se pudo obtener el partido {partidoId}: {(int)respuesta.StatusCode}");
        }

        using var documento = JsonDocument.Parse(contenido);
        return MapeoEstadisticasJson.APartido(documento.RootElement);
    }

    public async Task<List<EstadisticaSeleccion>> ObtenerEstadisticasSeleccionesAsync()
    {
        var json = await ObtenerJsonExitoAsync("estadisticas/selecciones", "listar estadísticas por selección");
        return MapearLista(json, MapeoEstadisticasJson.AEstadistica);
    }

    private void RegistrarMuestraBanderas(List<Partido> partidos)
    {
        var muestra = partidos
            .SelectMany(p => new[] { p.SeleccionLocal, p.SeleccionVisitante })
            .Where(s => s is not null && !string.IsNullOrWhiteSpace(s.Nombre))
            .GroupBy(s => s.CodigoPais ?? s.Nombre)
            .Select(g => g.First())
            .Take(8)
            .ToList();

        if (muestra.Count == 0)
        {
            _bitacora.LogWarning("Banderas: no hay selecciones en los partidos mapeados.");
            return;
        }

        var vacias = muestra.Count(s => string.IsNullOrWhiteSpace(s.Bandera));
        _bitacora.LogInformation(
            "Banderas: {TotalPartidos} partidos, muestra {Muestra} selecciones ({Vacias} sin bandera)",
            partidos.Count,
            muestra.Count,
            vacias);

        foreach (var seleccion in muestra)
        {
            _bitacora.LogInformation("Bandera → {Detalle}", MapeoEstadisticasJson.DescribirBandera(seleccion));
        }
    }

    private async Task<JsonElement> ObtenerJsonExitoAsync(string ruta, string operacion)
    {
        using var respuesta = await _http.GetAsync(ruta);
        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al {Operacion}: {Contenido}",
                (int)respuesta.StatusCode,
                operacion,
                contenido);
            throw new HttpRequestException($"Error al {operacion}: {(int)respuesta.StatusCode}");
        }

        using var documento = JsonDocument.Parse(contenido);
        return documento.RootElement.Clone();
    }

    private static List<T> MapearLista<T>(JsonElement json, Func<JsonElement, T?> mapear) where T : class
    {
        if (json.ValueKind != JsonValueKind.Array)
        {
            return new List<T>();
        }

        var lista = new List<T>();
        foreach (var elemento in json.EnumerateArray())
        {
            var mapeado = mapear(elemento);
            if (mapeado is not null)
            {
                lista.Add(mapeado);
            }
        }

        return lista;
    }
}
