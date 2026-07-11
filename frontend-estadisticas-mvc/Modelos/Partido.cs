namespace FrontendEstadisticas.Modelos;

public class Partido
{
    public long Id { get; set; }
    public Seleccion SeleccionLocal { get; set; } = new();
    public Seleccion SeleccionVisitante { get; set; } = new();
    public DateTime FechaHora { get; set; }
    public string Sede { get; set; } = string.Empty;
    public FaseTorneo Fase { get; set; }
    public string? Grupo { get; set; }
    public EstadoPartido Estado { get; set; }
    public int? GolesLocal { get; set; }
    public int? GolesVisitante { get; set; }
    public CuotasPartido Cuotas { get; set; } = new();

    public string Marcador =>
        GolesLocal.HasValue && GolesVisitante.HasValue
            ? $"{GolesLocal} - {GolesVisitante}"
            : "vs";

    public bool AdmitePredicciones => Estado == EstadoPartido.Programado && FechaHora > DateTime.Now;
}

public class CuotasPartido
{
    public decimal Local { get; set; }
    public decimal Empate { get; set; }
    public decimal Visitante { get; set; }

    public decimal Para(Pronostico pronostico) => pronostico switch
    {
        Pronostico.Local => Local,
        Pronostico.Empate => Empate,
        _ => Visitante
    };
}
