using System.Security.Claims;

namespace FrontendPublico.Extensiones;

public static class ExtensionesReclamos
{
    /// <summary>Identificador del usuario autenticado tomado de la cookie de sesión.</summary>
    public static long ObtenerUsuarioId(this ClaimsPrincipal usuario) =>
        long.Parse(usuario.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? throw new InvalidOperationException("La sesión no contiene el identificador del usuario."));

    public static string ObtenerNombre(this ClaimsPrincipal usuario) =>
        usuario.FindFirstValue(ClaimTypes.Name) ?? "Usuario";
}
