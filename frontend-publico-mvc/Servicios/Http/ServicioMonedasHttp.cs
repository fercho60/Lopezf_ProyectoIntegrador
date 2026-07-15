using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontendPublico.Modelos;
using FrontendPublico.Servicios;
using Microsoft.Extensions.Logging;

namespace FrontendPublico.Servicios.Http;

/// <summary>
/// Cliente HTTP hacia el Servicio UTNGolCoin.
/// Base: http://localhost:5000/api/
/// </summary>
public class ServicioMonedasHttp : IServicioMonedas
{
    private readonly HttpClient _http;
    private readonly ILogger<ServicioMonedasHttp> _bitacora;

    public ServicioMonedasHttp(HttpClient http, ILogger<ServicioMonedasHttp> bitacora)
    {
        _http = http;
        _bitacora = bitacora;

        if (_http.BaseAddress is null)
        {
            _http.BaseAddress = new Uri("http://localhost:5000/api/");
        }
    }

    public async Task<decimal> ObtenerSaldoAsync(long usuarioId)
    {
        var json = await ObtenerJsonExitoAsync($"billeteras/{usuarioId}", "consultar saldo");
        if (json.ValueKind == JsonValueKind.Number && json.TryGetDecimal(out var saldoDirecto))
        {
            return saldoDirecto;
        }

        if (json.TryGetProperty("saldo", out var saldoPropiedad) && saldoPropiedad.TryGetDecimal(out var saldo))
        {
            return saldo;
        }

        return 0m;
    }

    public async Task<List<Transaccion>> ObtenerTransaccionesAsync(long usuarioId)
    {
        var json = await ObtenerJsonExitoAsync($"billeteras/{usuarioId}/transacciones", "listar transacciones");
        return MapearLista(json, ATransaccion);
    }

    public async Task<Prediccion> CrearPrediccionAsync(long usuarioId, long partidoId, Pronostico pronostico, decimal monto)
    {
        var cuerpo = new
        {
            usuarioId,
            partidoId,
            resultadoPronosticado = PronosticoAApi(pronostico),
            monto
        };

        using var respuesta = await _http.PostAsJsonAsync("predicciones", cuerpo, MapeoEstadisticasJson.Opciones);
        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (respuesta.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.UnprocessableEntity)
        {
            throw LeerExcepcionReglaNegocio(contenido);
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al crear predicción: {Contenido}",
                (int)respuesta.StatusCode,
                contenido);
            throw new HttpRequestException($"No se pudo crear la predicción: {(int)respuesta.StatusCode}");
        }

        using var documento = JsonDocument.Parse(contenido);
        return APrediccion(documento.RootElement, usuarioId)
               ?? throw new HttpRequestException("La respuesta de predicción no es válida.");
    }

    public async Task<List<Prediccion>> ObtenerPrediccionesAsync(long usuarioId)
    {
        var json = await ObtenerJsonExitoAsync($"predicciones/usuario/{usuarioId}", "listar predicciones");
        return MapearLista(json, elemento => APrediccion(elemento, usuarioId));
    }

    public async Task<List<EntradaRanking>> ObtenerRankingAsync()
    {
        var json = await ObtenerJsonExitoAsync("ranking", "obtener ranking");
        if (json.ValueKind != JsonValueKind.Array)
        {
            return new List<EntradaRanking>();
        }

        var ranking = new List<EntradaRanking>();
        var posicion = 1;

        foreach (var elemento in json.EnumerateArray())
        {
            var usuarioId = elemento.TryGetProperty("usuarioId", out var usuarioIdJson) && usuarioIdJson.TryGetInt64(out var id)
                ? id
                : PropiedadLong(elemento, "id");

            var saldo = elemento.TryGetProperty("saldo", out var saldoJson) && saldoJson.TryGetDecimal(out var saldoValor)
                ? saldoValor
                : 0m;

            ranking.Add(new EntradaRanking
            {
                Posicion = posicion++,
                NombreUsuario = $"Usuario {usuarioId}",
                Saldo = saldo,
                Predicciones = 0,
                Aciertos = 0
            });
        }

        return ranking;
    }

    public async Task<bool> ReclamarBonoDiarioAsync(long usuarioId)
    {
        using var respuesta = await _http.PostAsync($"predicciones/bono-diario/{usuarioId}", null);
        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al reclamar bono diario: {Contenido}",
                (int)respuesta.StatusCode,
                contenido);
            throw new HttpRequestException($"No se pudo reclamar el bono diario: {(int)respuesta.StatusCode}");
        }

        if (bool.TryParse(contenido, out var otorgadoDirecto))
        {
            return otorgadoDirecto;
        }

        using var documento = JsonDocument.Parse(contenido);
        var raiz = documento.RootElement;

        if (raiz.ValueKind == JsonValueKind.True)
        {
            return true;
        }

        if (raiz.ValueKind == JsonValueKind.False)
        {
            return false;
        }

        if (raiz.TryGetProperty("otorgado", out var otorgado) && otorgado.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            return otorgado.GetBoolean();
        }

        return false;
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

    private static Transaccion? ATransaccion(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return new Transaccion
        {
            Id = PropiedadLong(el, "id"),
            Tipo = ParseTipoTransaccion(PropiedadCadenaOpcional(el, "tipo")),
            Concepto = PropiedadCadena(el, "concepto"),
            Monto = PropiedadDecimal(el, "monto"),
            FechaHora = ParseFechaHora(PropiedadCadena(el, "fechaHora"))
        };
    }

    private static Prediccion? APrediccion(JsonElement el, long usuarioIdPredeterminado)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var partidoId = PropiedadLong(el, "partidoId");
        var descripcion = PropiedadCadenaOpcional(el, "descripcionPartido");

        return new Prediccion
        {
            Id = PropiedadLong(el, "id"),
            UsuarioId = el.TryGetProperty("usuarioId", out var usuarioIdJson) && usuarioIdJson.TryGetInt64(out var usuarioId)
                ? usuarioId
                : usuarioIdPredeterminado,
            PartidoId = partidoId,
            DescripcionPartido = string.IsNullOrWhiteSpace(descripcion) ? $"Partido #{partidoId}" : descripcion,
            Pronostico = ParsePronostico(
                PropiedadCadenaOpcional(el, "resultadoPronosticado")
                ?? PropiedadCadenaOpcional(el, "pronostico")),
            Monto = PropiedadDecimal(el, "monto"),
            CuotaAplicada = PropiedadDecimal(el, "cuotaAplicada"),
            Estado = ParseEstadoPrediccion(PropiedadCadenaOpcional(el, "estado")),
            PremioPagado = PropiedadDecimalNullable(el, "premioPagado"),
            FechaHora = ParseFechaHora(PropiedadCadena(el, "fechaHora"))
        };
    }

    private static string PronosticoAApi(Pronostico pronostico) => pronostico switch
    {
        Pronostico.Local => "LOCAL",
        Pronostico.Empate => "EMPATE",
        _ => "VISITANTE"
    };

    private static Pronostico ParsePronostico(string? valor) => valor?.Trim().ToUpperInvariant() switch
    {
        "LOCAL" => Pronostico.Local,
        "EMPATE" => Pronostico.Empate,
        "VISITANTE" => Pronostico.Visitante,
        _ => Pronostico.Local
    };

    private static EstadoPrediccion ParseEstadoPrediccion(string? valor) => valor?.Trim().ToUpperInvariant() switch
    {
        "PENDIENTE" => EstadoPrediccion.Pendiente,
        "GANADA" => EstadoPrediccion.Ganada,
        "PERDIDA" => EstadoPrediccion.Perdida,
        _ => EstadoPrediccion.Pendiente
    };

    private static TipoTransaccion ParseTipoTransaccion(string? valor) => valor?.Trim().ToUpperInvariant() switch
    {
        "BONO_BIENVENIDA" => TipoTransaccion.BonoBienvenida,
        "PREDICCION" => TipoTransaccion.Prediccion,
        "PREMIO" => TipoTransaccion.Premio,
        "BONO_DIARIO" => TipoTransaccion.BonoDiario,
        _ => TipoTransaccion.Prediccion
    };

    private static ExcepcionReglaNegocio LeerExcepcionReglaNegocio(string json)
    {
        try
        {
            using var documento = JsonDocument.Parse(json);
            var raiz = documento.RootElement;
            var codigo = raiz.TryGetProperty("codigo", out var codigoJson)
                ? codigoJson.GetString() ?? "ERROR_NEGOCIO"
                : "ERROR_NEGOCIO";
            var mensaje = raiz.TryGetProperty("mensaje", out var mensajeJson)
                ? mensajeJson.GetString() ?? "No se pudo completar la operación."
                : "No se pudo completar la operación.";

            codigo = codigo.ToUpperInvariant() switch
            {
                var c when c.Contains("SALDO") => "SALDO_INSUFICIENTE",
                var c when c.Contains("DUPLIC") => "PREDICCION_DUPLICADA",
                var c when c.Contains("INICI") || c.Contains("CERR") => "PARTIDO_INICIADO",
                _ => codigo
            };

            return new ExcepcionReglaNegocio(codigo, mensaje);
        }
        catch (JsonException)
        {
            return new ExcepcionReglaNegocio("ERROR_NEGOCIO", "No se pudo completar la operación.");
        }
    }

    private static DateTime ParseFechaHora(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return DateTime.MinValue;
        }

        if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var fecha))
        {
            return fecha;
        }

        return DateTime.TryParse(valor, out fecha) ? fecha : DateTime.MinValue;
    }

    private static string PropiedadCadena(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.ValueKind == JsonValueKind.String
            ? propiedad.GetString() ?? string.Empty
            : string.Empty;

    private static string? PropiedadCadenaOpcional(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.ValueKind == JsonValueKind.String
            ? propiedad.GetString()
            : null;

    private static long PropiedadLong(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.TryGetInt64(out var valor)
            ? valor
            : 0;

    private static decimal PropiedadDecimal(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.TryGetDecimal(out var valor)
            ? valor
            : 0m;

    private static decimal? PropiedadDecimalNullable(JsonElement el, string nombre)
    {
        if (!el.TryGetProperty(nombre, out var propiedad) || propiedad.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return propiedad.TryGetDecimal(out var valor) ? valor : null;
    }
}
