using System.Globalization;
using System.Text.Json;
using FrontendEstadisticas.Modelos;

namespace FrontendEstadisticas.Servicios.Http;

/// <summary>
/// Convierte respuestas JSON del Servicio de Estadísticas (Guacales, camelCase Jackson)
/// en modelos del frontend de estadísticas.
/// </summary>
public static class MapeoEstadisticasJson
{
    public static readonly JsonSerializerOptions Opciones = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static EstadoPartido ParseEstado(string? s) => s?.Trim().ToUpperInvariant() switch
    {
        "PROGRAMADO" => EstadoPartido.Programado,
        "EN_JUEGO" => EstadoPartido.EnJuego,
        "FINALIZADO" => EstadoPartido.Finalizado,
        _ => EstadoPartido.Programado
    };

    public static string EstadoAQuery(EstadoPartido e) => e switch
    {
        EstadoPartido.Programado => "PROGRAMADO",
        EstadoPartido.EnJuego => "EN_JUEGO",
        EstadoPartido.Finalizado => "FINALIZADO",
        _ => "PROGRAMADO"
    };

    public static FaseTorneo ParseFase(string? s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return FaseTorneo.FaseDeGrupos;
        }

        var normalizado = NormalizarTextoFase(s);

        if (normalizado.Contains("GRUPO"))
        {
            return FaseTorneo.FaseDeGrupos;
        }

        if (normalizado.Contains("DIECISEIS") || normalizado.Contains("16AV"))
        {
            return FaseTorneo.Dieciseisavos;
        }

        if (normalizado.Contains("OCTAV"))
        {
            return FaseTorneo.Octavos;
        }

        if (normalizado.Contains("CUART"))
        {
            return FaseTorneo.Cuartos;
        }

        if (normalizado.Contains("SEMIF"))
        {
            return FaseTorneo.Semifinal;
        }

        if (normalizado.Contains("TERCER") || normalizado.Contains("3ER") || normalizado.Contains("3RO"))
        {
            return FaseTorneo.TercerPuesto;
        }

        if (normalizado.Contains("FINAL"))
        {
            return FaseTorneo.Final;
        }

        return normalizado switch
        {
            "FASE_DE_GRUPOS" => FaseTorneo.FaseDeGrupos,
            "DIECISEISAVOS" => FaseTorneo.Dieciseisavos,
            "OCTAVOS" => FaseTorneo.Octavos,
            "CUARTOS" => FaseTorneo.Cuartos,
            "SEMIFINAL" => FaseTorneo.Semifinal,
            "TERCER_PUESTO" => FaseTorneo.TercerPuesto,
            "FINAL" => FaseTorneo.Final,
            _ => FaseTorneo.FaseDeGrupos
        };
    }

    public static Seleccion ASeleccion(JsonElement el)
    {
        var seleccion = new Seleccion
        {
            Id = PropiedadLong(el, "id"),
            Nombre = PropiedadCadena(el, "nombre"),
            CodigoPais = PropiedadCadena(el, "codigoPais"),
            Grupo = PropiedadCadena(el, "grupo")
        };

        seleccion.Bandera = el.TryGetProperty("bandera", out var bandera)
            ? bandera.GetString() ?? string.Empty
            : BanderaDesdeCodigoPais(seleccion.CodigoPais);

        return seleccion;
    }

    public static Partido? APartido(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var partido = new Partido
        {
            Id = PropiedadLong(el, "id"),
            SeleccionLocal = el.TryGetProperty("seleccionLocal", out var local)
                ? ASeleccion(local)
                : new Seleccion(),
            SeleccionVisitante = el.TryGetProperty("seleccionVisitante", out var visitante)
                ? ASeleccion(visitante)
                : new Seleccion(),
            FechaHora = ParseFechaHora(PropiedadCadena(el, "fechaHora")),
            Sede = PropiedadCadena(el, "sede"),
            Fase = ParseFase(PropiedadCadenaOpcional(el, "fase")),
            Grupo = PropiedadCadenaOpcional(el, "grupo"),
            Estado = ParseEstado(PropiedadCadenaOpcional(el, "estado")),
            GolesLocal = PropiedadEnteroNullable(el, "golesLocal"),
            GolesVisitante = PropiedadEnteroNullable(el, "golesVisitante"),
            Cuotas = el.TryGetProperty("cuotas", out var cuotas)
                ? new CuotasPartido
                {
                    Local = PropiedadDecimal(cuotas, "local"),
                    Empate = PropiedadDecimal(cuotas, "empate"),
                    Visitante = PropiedadDecimal(cuotas, "visitante")
                }
                : new CuotasPartido()
        };

        return partido;
    }

    public static Grupo? AGrupo(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        var grupo = new Grupo
        {
            Id = PropiedadLong(el, "id"),
            Nombre = PropiedadCadena(el, "nombre"),
            Posiciones = new List<PosicionEquipo>()
        };

        if (el.TryGetProperty("posiciones", out var posiciones) && posiciones.ValueKind == JsonValueKind.Array)
        {
            foreach (var posicion in posiciones.EnumerateArray())
            {
                var fila = APosicionEquipo(posicion);
                if (fila is not null)
                {
                    grupo.Posiciones.Add(fila);
                }
            }
        }

        return grupo;
    }

    public static EstadisticaSeleccion? AEstadistica(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return new EstadisticaSeleccion
        {
            Seleccion = el.TryGetProperty("seleccion", out var seleccion)
                ? ASeleccion(seleccion)
                : new Seleccion(),
            Jugados = PropiedadEntero(el, "jugados"),
            Ganados = PropiedadEntero(el, "ganados"),
            Empatados = PropiedadEntero(el, "empatados"),
            Perdidos = PropiedadEntero(el, "perdidos"),
            GolesFavor = PropiedadEntero(el, "golesFavor"),
            GolesContra = PropiedadEntero(el, "golesContra")
        };
    }

    private static PosicionEquipo? APosicionEquipo(JsonElement el)
    {
        if (el.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        return new PosicionEquipo
        {
            Seleccion = el.TryGetProperty("seleccion", out var seleccion)
                ? ASeleccion(seleccion)
                : new Seleccion(),
            Jugados = PropiedadEntero(el, "jugados"),
            Ganados = PropiedadEntero(el, "ganados"),
            Empatados = PropiedadEntero(el, "empatados"),
            Perdidos = PropiedadEntero(el, "perdidos"),
            GolesFavor = PropiedadEntero(el, "golesFavor"),
            GolesContra = PropiedadEntero(el, "golesContra"),
            Puntos = PropiedadEntero(el, "puntos")
        };
    }

    private static string NormalizarTextoFase(string s) =>
        s.Trim().ToUpperInvariant()
            .Replace(" ", "_")
            .Replace("Á", "A")
            .Replace("É", "E")
            .Replace("Í", "I")
            .Replace("Ó", "O")
            .Replace("Ú", "U");

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

    private static string BanderaDesdeCodigoPais(string codigoPais)
    {
        if (string.IsNullOrWhiteSpace(codigoPais) || codigoPais.Length != 2)
        {
            return string.Empty;
        }

        var codigo = codigoPais.ToUpperInvariant();
        if (!char.IsAsciiLetterUpper(codigo[0]) || !char.IsAsciiLetterUpper(codigo[1]))
        {
            return string.Empty;
        }

        return string.Concat(
            char.ConvertFromUtf32(0x1F1E6 + (codigo[0] - 'A')),
            char.ConvertFromUtf32(0x1F1E6 + (codigo[1] - 'A')));
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

    private static int PropiedadEntero(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.TryGetInt32(out var valor)
            ? valor
            : 0;

    private static int? PropiedadEnteroNullable(JsonElement el, string nombre)
    {
        if (!el.TryGetProperty(nombre, out var propiedad) || propiedad.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return propiedad.TryGetInt32(out var valor) ? valor : null;
    }

    private static decimal PropiedadDecimal(JsonElement el, string nombre) =>
        el.TryGetProperty(nombre, out var propiedad) && propiedad.TryGetDecimal(out var valor)
            ? valor
            : 0m;
}
