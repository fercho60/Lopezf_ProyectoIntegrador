namespace FrontendPublico.Modelos;

public class Grupo
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    /// <summary>Tabla de posiciones ordenada de mejor a peor.</summary>
    public List<PosicionEquipo> Posiciones { get; set; } = new();
}

public class PosicionEquipo
{
    public Seleccion Seleccion { get; set; } = new();
    public int Jugados { get; set; }
    public int Ganados { get; set; }
    public int Empatados { get; set; }
    public int Perdidos { get; set; }
    public int GolesFavor { get; set; }
    public int GolesContra { get; set; }
    public int Puntos { get; set; }
    public int DiferenciaGoles => GolesFavor - GolesContra;
}
