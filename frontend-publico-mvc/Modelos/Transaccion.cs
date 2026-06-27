namespace FrontendPublico.Modelos;

public class Transaccion
{
    public long Id { get; set; }
    public TipoTransaccion Tipo { get; set; }
    public string Concepto { get; set; } = string.Empty;
    /// <summary>Positivo acredita, negativo debita.</summary>
    public decimal Monto { get; set; }
    public DateTime FechaHora { get; set; }
}
