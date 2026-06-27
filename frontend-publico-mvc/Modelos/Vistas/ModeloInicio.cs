namespace FrontendPublico.Modelos.Vistas;

public class ModeloInicio
{
    public List<Partido> PartidosEnJuego { get; set; } = new();
    public List<Partido> ProximosPartidos { get; set; } = new();
    public List<EntradaRanking> TopRanking { get; set; } = new();
    /// <summary>Verdadero si el servicio de estadísticas no respondió (degradación controlada).</summary>
    public bool EstadisticasNoDisponibles { get; set; }
    public bool MonedasNoDisponibles { get; set; }
}
