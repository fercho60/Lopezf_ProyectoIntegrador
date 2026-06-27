using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios.Simulados;

/// <summary>
/// Implementación simulada de la autenticación centralizada en el Servicio de Estadísticas.
/// Las contraseñas se guardan solo como hash (RNF04); el hash definitivo lo hará el backend.
/// </summary>
public class ServicioAutenticacionSimulado : IServicioAutenticacion
{
    private readonly AlmacenDatosSimulados _almacen;

    public ServicioAutenticacionSimulado(AlmacenDatosSimulados almacen)
    {
        _almacen = almacen;
    }

    public Task<Usuario> RegistrarAsync(string nombre, string correo, string contrasena)
    {
        lock (_almacen.Candado)
        {
            var correoNormalizado = correo.Trim().ToLowerInvariant();
            if (_almacen.Usuarios.Any(u => u.Correo.Equals(correoNormalizado, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ExcepcionReglaNegocio("CORREO_DUPLICADO", "El correo ya está registrado.");
            }

            var usuario = new UsuarioConCredenciales
            {
                Id = _almacen.SiguienteUsuarioId(),
                Nombre = nombre.Trim(),
                Correo = correoNormalizado,
                Rol = "usuario",
                HashContrasena = AlmacenDatosSimulados.CalcularHash(contrasena)
            };
            _almacen.Usuarios.Add(usuario);

            // El registro dispara la creación de la billetera con el bono de bienvenida
            // de 10 UTNGolCoin registrado como transacción del ledger (RF01).
            _almacen.Saldos[usuario.Id] = 10;
            _almacen.Transacciones[usuario.Id] = new List<Transaccion>
            {
                new()
                {
                    Id = _almacen.SiguienteTransaccionId(),
                    Tipo = TipoTransaccion.BonoBienvenida,
                    Concepto = "Bono de bienvenida a UTN GolMundial 2026",
                    Monto = 10,
                    FechaHora = DateTime.Now
                }
            };

            return Task.FromResult<Usuario>(usuario);
        }
    }

    public Task<Usuario?> IniciarSesionAsync(string correo, string contrasena)
    {
        lock (_almacen.Candado)
        {
            var hash = AlmacenDatosSimulados.CalcularHash(contrasena);
            var usuario = _almacen.Usuarios.FirstOrDefault(u =>
                u.Correo.Equals(correo.Trim(), StringComparison.OrdinalIgnoreCase) &&
                u.HashContrasena == hash);
            return Task.FromResult<Usuario?>(usuario);
        }
    }
}
