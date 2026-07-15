using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FrontendPublico.Modelos;
using FrontendPublico.Servicios;
using Microsoft.Extensions.Logging;

namespace FrontendPublico.Servicios.Http;

/// <summary>
/// Cliente HTTP de autenticación centralizada en el Servicio de Estadísticas (Guacales).
/// </summary>
public class ServicioAutenticacionHttp : IServicioAutenticacion
{
    private readonly HttpClient _http;
    private readonly ILogger<ServicioAutenticacionHttp> _bitacora;

    public ServicioAutenticacionHttp(HttpClient http, ILogger<ServicioAutenticacionHttp> bitacora)
    {
        _http = http;
        _bitacora = bitacora;

        if (_http.BaseAddress is null)
        {
            _http.BaseAddress = new Uri("http://localhost:8080/demo/api/v1/");
        }
    }

    public async Task<Usuario> RegistrarAsync(string nombre, string correo, string contrasena)
    {
        var cuerpo = new
        {
            nombre = nombre.Trim(),
            correo = correo.Trim(),
            contrasena
        };

        using var respuesta = await _http.PostAsJsonAsync("autenticacion/registro", cuerpo, MapeoEstadisticasJson.Opciones);
        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (respuesta.StatusCode == HttpStatusCode.Conflict)
        {
            throw LeerExcepcionReglaNegocio(contenido, "CORREO_DUPLICADO", "El correo ya está registrado.");
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al registrar usuario: {Contenido}",
                (int)respuesta.StatusCode,
                contenido);
            throw new HttpRequestException($"No se pudo registrar el usuario: {(int)respuesta.StatusCode}");
        }

        return LeerUsuarioSesion(contenido)
               ?? throw new HttpRequestException("La respuesta de registro no incluyó datos de usuario.");
    }

    public async Task<Usuario?> IniciarSesionAsync(string correo, string contrasena)
    {
        var cuerpo = new
        {
            correo = correo.Trim(),
            contrasena
        };

        using var respuesta = await _http.PostAsJsonAsync("autenticacion/sesion", cuerpo, MapeoEstadisticasJson.Opciones);
        var contenido = await respuesta.Content.ReadAsStringAsync();

        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        if (!respuesta.IsSuccessStatusCode)
        {
            _bitacora.LogError(
                "Error HTTP {Estado} al iniciar sesión: {Contenido}",
                (int)respuesta.StatusCode,
                contenido);
            throw new HttpRequestException($"No se pudo iniciar sesión: {(int)respuesta.StatusCode}");
        }

        return LeerUsuarioSesion(contenido);
    }

    private static Usuario? LeerUsuarioSesion(string json)
    {
        using var documento = JsonDocument.Parse(json);
        var raiz = documento.RootElement;

        if (!raiz.TryGetProperty("usuario", out var usuarioJson))
        {
            return null;
        }

        return new Usuario
        {
            Id = usuarioJson.TryGetProperty("id", out var id) && id.TryGetInt64(out var idValor) ? idValor : 0,
            Nombre = usuarioJson.TryGetProperty("nombre", out var nombre) ? nombre.GetString() ?? string.Empty : string.Empty,
            Correo = usuarioJson.TryGetProperty("correo", out var correo) ? correo.GetString() ?? string.Empty : string.Empty,
            Rol = usuarioJson.TryGetProperty("rol", out var rol) ? rol.GetString() ?? "usuario" : "usuario"
        };
    }

    private static ExcepcionReglaNegocio LeerExcepcionReglaNegocio(string json, string codigoPredeterminado, string mensajePredeterminado)
    {
        try
        {
            using var documento = JsonDocument.Parse(json);
            var raiz = documento.RootElement;
            var codigo = raiz.TryGetProperty("codigo", out var codigoJson)
                ? codigoJson.GetString() ?? codigoPredeterminado
                : codigoPredeterminado;
            var mensaje = raiz.TryGetProperty("mensaje", out var mensajeJson)
                ? mensajeJson.GetString() ?? mensajePredeterminado
                : mensajePredeterminado;
            return new ExcepcionReglaNegocio(codigo, mensaje);
        }
        catch (JsonException)
        {
            return new ExcepcionReglaNegocio(codigoPredeterminado, mensajePredeterminado);
        }
    }
}
