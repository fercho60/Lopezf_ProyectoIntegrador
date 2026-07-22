using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace FrontendPublico.Servicios.Http;

/// <summary>
/// Registra método, URL, payload de request y cuerpo de response de todas las
/// llamadas HttpClient a backends (Guacales / UTNGolCoin). Ver en la terminal del FE.
/// </summary>
public sealed class ManejadorHttpLog : DelegatingHandler
{
    private const int MaxCuerpoLog = 4000;
    private readonly ILogger<ManejadorHttpLog> _bitacora;

    public ManejadorHttpLog(ILogger<ManejadorHttpLog> bitacora)
    {
        _bitacora = bitacora;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var correlacion = Guid.NewGuid().ToString("N")[..8];
        var metodo = request.Method.Method;
        var url = request.RequestUri?.ToString() ?? "(sin-url)";
        var cuerpoRequest = await LeerCuerpoRequestAsync(request, cancellationToken);

        _bitacora.LogInformation(
            "HTTP→ [{Id}] {Metodo} {Url}{Payload}",
            correlacion,
            metodo,
            url,
            string.IsNullOrWhiteSpace(cuerpoRequest) ? string.Empty : $"\n  request: {Truncar(cuerpoRequest)}");

        var reloj = Stopwatch.StartNew();
        try
        {
            var respuesta = await base.SendAsync(request, cancellationToken);
            reloj.Stop();

            var cuerpoResponse = await respuesta.Content.ReadAsStringAsync(cancellationToken);
            var contentTypeOriginal = respuesta.Content.Headers.ContentType?.MediaType;
            respuesta.Content = CrearContent(cuerpoResponse, contentTypeOriginal);

            _bitacora.LogInformation(
                "HTTP← [{Id}] {Estado} ({Ms} ms) {Metodo} {Url}\n  response: {Cuerpo}",
                correlacion,
                (int)respuesta.StatusCode,
                reloj.ElapsedMilliseconds,
                metodo,
                url,
                Truncar(cuerpoResponse));

            return respuesta;
        }
        catch (Exception excepcion)
        {
            reloj.Stop();
            _bitacora.LogError(
                excepcion,
                "HTTP✗ [{Id}] FALLÓ ({Ms} ms) {Metodo} {Url}",
                correlacion,
                reloj.ElapsedMilliseconds,
                metodo,
                url);
            throw;
        }
    }

    private static async Task<string> LeerCuerpoRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return string.Empty;
        }

        var texto = await request.Content.ReadAsStringAsync(cancellationToken);
        var mediaType = request.Content.Headers.ContentType?.MediaType;
        request.Content = CrearContent(texto, mediaType);
        return texto;
    }

    private static StringContent CrearContent(string texto, string? mediaType)
    {
        var content = new StringContent(texto, Encoding.UTF8);
        content.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrWhiteSpace(mediaType) ? "application/json" : mediaType);
        return content;
    }

    private static string Truncar(string texto)
    {
        if (string.IsNullOrEmpty(texto))
        {
            return "(vacío)";
        }

        var unaLinea = texto.Replace("\r", " ").Replace("\n", " ");
        return unaLinea.Length <= MaxCuerpoLog
            ? unaLinea
            : unaLinea[..MaxCuerpoLog] + $"…(+{unaLinea.Length - MaxCuerpoLog} chars)";
    }
}
