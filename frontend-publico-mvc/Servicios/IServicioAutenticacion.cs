using FrontendPublico.Modelos;

namespace FrontendPublico.Servicios;

/// <summary>
/// Puerto de autenticación, centralizada en el Servicio de Estadísticas.
/// Contrato: docs/contratos/estadisticas-openapi.yaml (/autenticacion/*)
/// </summary>
public interface IServicioAutenticacion
{
    /// <summary>
    /// Registra un usuario nuevo. Dispara la creación de la billetera con el bono
    /// de bienvenida de 10 UTNGolCoin. Lanza <see cref="ExcepcionReglaNegocio"/>
    /// con código CORREO_DUPLICADO si el correo ya existe.
    /// </summary>
    Task<Usuario> RegistrarAsync(string nombre, string correo, string contrasena);

    /// <summary>Devuelve el usuario si las credenciales son válidas; nulo en caso contrario.</summary>
    Task<Usuario?> IniciarSesionAsync(string correo, string contrasena);
}
