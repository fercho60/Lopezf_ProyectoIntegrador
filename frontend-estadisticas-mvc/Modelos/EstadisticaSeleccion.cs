namespace FrontendEstadisticas.Modelos;

public class EstadisticaSeleccion
{
    public Seleccion Seleccion { get; set; } = new();
    public int Jugados { get; set; }
    public int Ganados { get; set; }
    public int Empatados { get; set; }
    public int Perdidos { get; set; }
    public int GolesFavor { get; set; }
    public int GolesContra { get; set; }
}
