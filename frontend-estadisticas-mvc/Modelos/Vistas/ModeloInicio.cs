using FrontendEstadisticas.Modelos;

namespace FrontendEstadisticas.Modelos.Vistas;

public class ModeloInicio
{
    public List<Partido> PartidosEnJuego { get; set; } = new();
    public List<Partido> ProximosPartidos { get; set; } = new();
    public bool EstadisticasNoDisponibles { get; set; }
    public string UrlApuestas { get; set; } = "http://localhost:5081";
}
