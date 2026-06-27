namespace FrontendPublico.Modelos.Vistas;

public class ModeloCalendario
{
    public List<Partido> Partidos { get; set; } = new();
    public string? FiltroEstado { get; set; }
    public string? FiltroGrupo { get; set; }
    public List<string> GruposDisponibles { get; set; } = new();
}
