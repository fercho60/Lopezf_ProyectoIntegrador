using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios;

/// <summary>
/// Puerto hacia el Servicio UTNGolCoin (ASP.NET Core + MySQL).
/// Contrato: docs/contratos/utngolcoin-openapi.yaml
/// </summary>
public interface IServicioMonedas
{
    Task<decimal> ObtenerSaldoAsync(long usuarioId);
    Task<List<Transaccion>> ObtenerTransaccionesAsync(long usuarioId);

    /// <summary>
    /// Crea una predicción 1X2 debitando el monto de la billetera.
    /// Lanza <see cref="ExcepcionReglaNegocio"/> con códigos SALDO_INSUFICIENTE,
    /// PREDICCION_DUPLICADA o PARTIDO_INICIADO según la regla incumplida.
    /// </summary>
    Task<Prediccion> CrearPrediccionAsync(long usuarioId, long partidoId, Pronostico pronostico, decimal monto);

    Task<List<Prediccion>> ObtenerPrediccionesAsync(long usuarioId);
    Task<List<EntradaRanking>> ObtenerRankingAsync();

    /// <summary>Acredita 1 UTNGolCoin si el saldo es cero y no se reclamó hoy. Devuelve si se otorgó.</summary>
    Task<bool> ReclamarBonoDiarioAsync(long usuarioId);
}
