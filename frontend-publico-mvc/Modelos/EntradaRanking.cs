namespace FrontendPublico.Modelos;

public class EntradaRanking
{
    public int Posicion { get; set; }
    public string NombreUsuario { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
    public int Predicciones { get; set; }
    public int Aciertos { get; set; }

    public string Iniciales
    {
        get
        {
            var partes = NombreUsuario.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return partes.Length >= 2
                ? $"{partes[0][0]}{partes[1][0]}".ToUpperInvariant()
                : NombreUsuario.Length > 0 ? NombreUsuario[..1].ToUpperInvariant() : "?";
        }
    }
}
