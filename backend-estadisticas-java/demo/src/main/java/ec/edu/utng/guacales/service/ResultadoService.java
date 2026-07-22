package ec.edu.utng.guacales.service;

import ec.edu.utng.guacales.model.Partido;
import ec.edu.utng.guacales.model.Seleccion;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.transaction.Transactional;

@ApplicationScoped
public class ResultadoService {

    @PersistenceContext(unitName = "guacalesPU")
    private EntityManager em;

    @Inject
    private UtnGolCoinClient utnGolCoinClient;

    @Transactional
    public Partido registrarResultado(Long partidoId, Integer golesLocal, Integer golesVisitante) {
        Partido partido = em.find(Partido.class, partidoId);
        if (partido == null) {
            return null;
        }

        if ("FINALIZADO".equals(partido.getEstado())
                && partido.getGolesLocal() != null && partido.getGolesVisitante() != null) {
            aplicarEstadisticas(partido.getSeleccionLocal(), partido.getSeleccionVisitante(),
                    partido.getGolesLocal(), partido.getGolesVisitante(), -1);
        }

        partido.setGolesLocal(golesLocal);
        partido.setGolesVisitante(golesVisitante);
        partido.setEstado("FINALIZADO");
        em.merge(partido);

        aplicarEstadisticas(partido.getSeleccionLocal(), partido.getSeleccionVisitante(),
                golesLocal, golesVisitante, 1);

        // Notifica a UTNGolCoin para liquidar predicciones (RF12).
        utnGolCoinClient.liquidarPartido(partidoId, golesLocal, golesVisitante);

        return partido;
    }

    private void aplicarEstadisticas(Seleccion local, Seleccion visitante,
                                      int golesLocal, int golesVisitante, int factor) {
        local.setPartidosJugados(local.getPartidosJugados() + (1 * factor));
        visitante.setPartidosJugados(visitante.getPartidosJugados() + (1 * factor));

        local.setGolesFavor(local.getGolesFavor() + (golesLocal * factor));
        local.setGolesContra(local.getGolesContra() + (golesVisitante * factor));

        visitante.setGolesFavor(visitante.getGolesFavor() + (golesVisitante * factor));
        visitante.setGolesContra(visitante.getGolesContra() + (golesLocal * factor));

        if (golesLocal > golesVisitante) {
            local.setPartidosGanados(local.getPartidosGanados() + (1 * factor));
            local.setPuntos(local.getPuntos() + (3 * factor));
            visitante.setPartidosPerdidos(visitante.getPartidosPerdidos() + (1 * factor));
        } else if (golesLocal < golesVisitante) {
            visitante.setPartidosGanados(visitante.getPartidosGanados() + (1 * factor));
            visitante.setPuntos(visitante.getPuntos() + (3 * factor));
            local.setPartidosPerdidos(local.getPartidosPerdidos() + (1 * factor));
        } else {
            local.setPartidosEmpatados(local.getPartidosEmpatados() + (1 * factor));
            local.setPuntos(local.getPuntos() + (1 * factor));
            visitante.setPartidosEmpatados(visitante.getPartidosEmpatados() + (1 * factor));
            visitante.setPuntos(visitante.getPuntos() + (1 * factor));
        }

        em.merge(local);
        em.merge(visitante);
    }
}
