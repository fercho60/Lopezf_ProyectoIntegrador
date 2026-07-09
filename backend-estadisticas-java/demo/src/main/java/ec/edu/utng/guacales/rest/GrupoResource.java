package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.GrupoDTO;
import ec.edu.utng.guacales.dto.PosicionEquipoDTO;
import ec.edu.utng.guacales.model.Grupo;
import ec.edu.utng.guacales.model.Seleccion;
import ec.edu.utng.guacales.repository.SeleccionRepository;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.inject.Inject;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import java.util.List;
import java.util.stream.Collectors;

@Path("/grupos")
@Produces(MediaType.APPLICATION_JSON)
@ApplicationScoped
public class GrupoResource {

    @PersistenceContext(unitName = "guacalesPU")
    private EntityManager em;

    @Inject
    private SeleccionRepository seleccionRepository;

    @GET
    public List<GrupoDTO> listar() {
        List<Grupo> grupos = em.createQuery("SELECT g FROM Grupo g ORDER BY g.codigo", Grupo.class)
                .getResultList();
        return grupos.stream().map(this::convertir).collect(Collectors.toList());
    }

    private GrupoDTO convertir(Grupo g) {
        GrupoDTO dto = new GrupoDTO();
        dto.setId(g.getId());
        dto.setNombre(g.getNombre());

        List<Seleccion> selecciones = seleccionRepository.listarPorGrupo(g.getCodigo());
        List<PosicionEquipoDTO> posiciones = selecciones.stream().map(s -> {
            PosicionEquipoDTO p = new PosicionEquipoDTO();
            p.setSeleccion(SeleccionResource.convertir(s));
            p.setJugados(s.getPartidosJugados());
            p.setGanados(s.getPartidosGanados());
            p.setEmpatados(s.getPartidosEmpatados());
            p.setPerdidos(s.getPartidosPerdidos());
            p.setGolesFavor(s.getGolesFavor());
            p.setGolesContra(s.getGolesContra());
            p.setPuntos(s.getPuntos());
            return p;
        }).collect(Collectors.toList());

        dto.setPosiciones(posiciones);
        return dto;
    }
}
