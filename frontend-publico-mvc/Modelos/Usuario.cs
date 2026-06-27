namespace FrontendPublico.Modelos;

public class Usuario
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    /// <summary>Rol del sistema: administrador, usuario o invitado.</summary>
    public string Rol { get; set; } = "usuario";
}
