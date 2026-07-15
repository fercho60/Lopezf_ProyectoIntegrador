using System.Text.Json;
using FrontendPublico.Modelos;
using FrontendPublico.Servicios;
using Microsoft.Extensions.Logging;

namespace FrontendPublico.Servicios.Http;

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
        var consulta = new List<string>();
        if (estado.HasValue)
        {
            consulta.Add($"estado={Uri.EscapeDataString(MapeoEstadisticasJson.EstadoAQuery(estado.Value))}");
        }

        if (!string.IsNullOrWhiteSpace(grupo))
        {
            consulta.Add($"grupo={Uri.EscapeDataString(grupo)}");
        }

        var ruta = consulta.Count > 0
            ? $"partidos?{string.Join("&", consulta)}"
            : "partidos";

        var json = await ObtenerJsonExitoAsync(ruta, "listar partidos");
        return MapearLista(json, MapeoEstadisticasJson.APartido);
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
