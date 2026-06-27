namespace FrontendPublico.Servicios;

/// <summary>
/// Error de negocio devuelto por un servicio (equivale a las respuestas 409/422 del contrato).
/// El mensaje es apto para mostrarse directamente al usuario.
/// </summary>
public class ExcepcionReglaNegocio : Exception
{
    public string Codigo { get; }

    public ExcepcionReglaNegocio(string codigo, string mensaje) : base(mensaje)
    {
        Codigo = codigo;
    }
}
