namespace FrontendPublico.Modelos;

public class Seleccion
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Código FIFA (3) o ISO 3166-1 alfa-2 recibido de la API.</summary>
    public string CodigoPais { get; set; } = string.Empty;
    /// <summary>ISO 3166-1 alfa-2 para imágenes de bandera (flagcdn). Windows no pinta emoji de banderas.</summary>
    public string CodigoIso2 { get; set; } = string.Empty;
    /// <summary>Bandera en emoji (fallback; en Windows suele verse como letras MX/ZA).</summary>
    public string Bandera { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;

    /// <summary>URL de bandera PNG (48×36). Vacío si no hay código ISO válido.</summary>
    public string UrlBandera =>
        string.IsNullOrWhiteSpace(CodigoIso2)
            ? string.Empty
            : $"https://flagcdn.com/w80/{CodigoIso2.ToLowerInvariant()}.png";
}
