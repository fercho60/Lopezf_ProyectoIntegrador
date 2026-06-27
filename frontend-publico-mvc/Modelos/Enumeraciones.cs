namespace FrontendPublico.Modelos;

public enum EstadoPartido
{
    Programado,
    EnJuego,
    Finalizado
}

public enum FaseTorneo
{
    FaseDeGrupos,
    Dieciseisavos,
    Octavos,
    Cuartos,
    Semifinal,
    TercerPuesto,
    Final
}

public enum TipoTransaccion
{
    BonoBienvenida,
    Prediccion,
    Premio,
    BonoDiario
}

public enum Pronostico
{
    Local,
    Empate,
    Visitante
}

public enum EstadoPrediccion
{
    Pendiente,
    Ganada,
    Perdida
}
