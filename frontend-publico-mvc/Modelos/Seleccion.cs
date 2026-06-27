namespace FrontendPublico.Modelos;

public class Seleccion
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Código ISO 3166-1 alfa-2, usado para mostrar la bandera.</summary>
    public string CodigoPais { get; set; } = string.Empty;
    /// <summary>Bandera en emoji para las vistas.</summary>
    public string Bandera { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;
}
