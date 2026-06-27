namespace FrontendPublico.Modelos.Vistas;

public class ModeloBilletera
{
    public decimal Saldo { get; set; }
    public List<Transaccion> Transacciones { get; set; } = new();
}
