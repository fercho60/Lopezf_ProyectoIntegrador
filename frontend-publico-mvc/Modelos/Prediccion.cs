namespace FrontendPublico.Modelos;

public class Prediccion
{
    public long Id { get; set; }
    public long UsuarioId { get; set; }
    public long PartidoId { get; set; }
    public string DescripcionPartido { get; set; } = string.Empty;
    public Pronostico Pronostico { get; set; }
    public decimal Monto { get; set; }
    /// <summary>Cuota congelada al momento de crear la predicción.</summary>
    public decimal CuotaAplicada { get; set; }
    public EstadoPrediccion Estado { get; set; }
    /// <summary>Monto por cuota, solo cuando la predicción resulta ganada.</summary>
    public decimal? PremioPagado { get; set; }
    public DateTime FechaHora { get; set; }

    public decimal PremioPotencial => Math.Round(Monto * CuotaAplicada, 2);
}
