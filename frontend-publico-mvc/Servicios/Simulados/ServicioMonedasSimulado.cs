using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios.Simulados;

/// <summary>
/// Implementación simulada del Servicio UTNGolCoin. Se sustituirá por un cliente
/// HTTP contra docs/contratos/utngolcoin-openapi.yaml cuando el backend exista.
/// </summary>
public class ServicioMonedasSimulado : IServicioMonedas
{
    private readonly AlmacenDatosSimulados _almacen;

    public ServicioMonedasSimulado(AlmacenDatosSimulados almacen)
    {
        _almacen = almacen;
    }

    public Task<decimal> ObtenerSaldoAsync(long usuarioId)
    {
        lock (_almacen.Candado)
        {
            return Task.FromResult(_almacen.Saldos.GetValueOrDefault(usuarioId, 0));
        }
    }

    public Task<List<Transaccion>> ObtenerTransaccionesAsync(long usuarioId)
    {
        lock (_almacen.Candado)
        {
            var transacciones = _almacen.Transacciones.GetValueOrDefault(usuarioId, new List<Transaccion>())
                .OrderByDescending(t => t.FechaHora)
                .ToList();
            return Task.FromResult(transacciones);
        }
    }

    public Task<Prediccion> CrearPrediccionAsync(long usuarioId, long partidoId, Pronostico pronostico, decimal monto)
    {
        lock (_almacen.Candado)
        {
            var partido = _almacen.Partidos.FirstOrDefault(p => p.Id == partidoId)
                ?? throw new ExcepcionReglaNegocio("PARTIDO_NO_ENCONTRADO", "El partido indicado no existe.");

            // RF17: cierre automático a la fecha y hora de inicio del partido
            if (!partido.AdmitePredicciones)
            {
                throw new ExcepcionReglaNegocio("PARTIDO_INICIADO",
                    "Las predicciones de este partido están cerradas: el encuentro ya inició o finalizó.");
            }

            // RF18: una única predicción por usuario y partido
            if (_almacen.Predicciones.Any(p => p.UsuarioId == usuarioId && p.PartidoId == partidoId))
            {
                throw new ExcepcionReglaNegocio("PREDICCION_DUPLICADA",
                    "Ya tienes una predicción registrada para este partido.");
            }

            if (monto < 1)
            {
                throw new ExcepcionReglaNegocio("MONTO_INVALIDO", "El monto mínimo es 1 UTNGolCoin.");
            }

            // RF16: validación de saldo suficiente
            var saldo = _almacen.Saldos.GetValueOrDefault(usuarioId, 0);
            if (monto > saldo)
            {
                throw new ExcepcionReglaNegocio("SALDO_INSUFICIENTE",
                    $"El saldo disponible ({saldo:0.##} UTNGolCoin) no cubre el monto de la predicción.");
            }

            var prediccion = new Prediccion
            {
                Id = _almacen.SiguientePrediccionId(),
                UsuarioId = usuarioId,
                PartidoId = partidoId,
                DescripcionPartido = $"{partido.SeleccionLocal.Nombre} vs {partido.SeleccionVisitante.Nombre}",
                Pronostico = pronostico,
                Monto = monto,
                CuotaAplicada = partido.Cuotas.Para(pronostico),
                Estado = EstadoPrediccion.Pendiente,
                FechaHora = DateTime.Now
            };
            _almacen.Predicciones.Add(prediccion);

            // Débito en el ledger
            _almacen.Saldos[usuarioId] = saldo - monto;
            ObtenerListaTransacciones(usuarioId).Add(new Transaccion
            {
                Id = _almacen.SiguienteTransaccionId(),
                Tipo = TipoTransaccion.Prediccion,
                Concepto = $"Predicción: {prediccion.DescripcionPartido}",
                Monto = -monto,
                FechaHora = DateTime.Now
            });

            return Task.FromResult(prediccion);
        }
    }

    public Task<List<Prediccion>> ObtenerPrediccionesAsync(long usuarioId)
    {
        lock (_almacen.Candado)
        {
            var predicciones = _almacen.Predicciones
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.FechaHora)
                .ToList();
            return Task.FromResult(predicciones);
        }
    }

    public Task<List<EntradaRanking>> ObtenerRankingAsync()
    {
        lock (_almacen.Candado)
        {
            var ranking = _almacen.Usuarios
                .Select(u => new
                {
                    Usuario = u,
                    Saldo = _almacen.Saldos.GetValueOrDefault(u.Id, 0),
                    Predicciones = _almacen.Predicciones.Count(p => p.UsuarioId == u.Id),
                    Aciertos = _almacen.Predicciones.Count(p => p.UsuarioId == u.Id && p.Estado == EstadoPrediccion.Ganada)
                })
                .OrderByDescending(x => x.Saldo)
                .ThenByDescending(x => x.Aciertos)
                .Select((x, indice) => new EntradaRanking
                {
                    Posicion = indice + 1,
                    NombreUsuario = x.Usuario.Nombre,
                    Saldo = x.Saldo,
                    Predicciones = x.Predicciones,
                    Aciertos = x.Aciertos
                })
                .ToList();
            return Task.FromResult(ranking);
        }
    }

    public Task<bool> ReclamarBonoDiarioAsync(long usuarioId)
    {
        lock (_almacen.Candado)
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var saldo = _almacen.Saldos.GetValueOrDefault(usuarioId, 0);
            var yaReclamado = _almacen.UltimoBonoDiario.TryGetValue(usuarioId, out var ultimaFecha) && ultimaFecha == hoy;

            // RF20: 1 UTNGolCoin diaria únicamente cuando el saldo es cero
            if (saldo != 0 || yaReclamado)
            {
                return Task.FromResult(false);
            }

            _almacen.Saldos[usuarioId] = 1;
            _almacen.UltimoBonoDiario[usuarioId] = hoy;
            ObtenerListaTransacciones(usuarioId).Add(new Transaccion
            {
                Id = _almacen.SiguienteTransaccionId(),
                Tipo = TipoTransaccion.BonoDiario,
                Concepto = "Bono diario anti-bancarrota",
                Monto = 1,
                FechaHora = DateTime.Now
            });
            return Task.FromResult(true);
        }
    }

    private List<Transaccion> ObtenerListaTransacciones(long usuarioId)
    {
        if (!_almacen.Transacciones.TryGetValue(usuarioId, out var lista))
        {
            lista = new List<Transaccion>();
            _almacen.Transacciones[usuarioId] = lista;
        }
        return lista;
    }
}
