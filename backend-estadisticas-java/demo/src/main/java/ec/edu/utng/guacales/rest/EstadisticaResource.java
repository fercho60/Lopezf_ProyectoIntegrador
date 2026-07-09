package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.EstadisticaSeleccionDTO;
import ec.edu.utng.guacales.model.Seleccion;
import ec.edu.utng.guacales.repository.SeleccionRepository;
import jakarta.inject.Inject;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import java.util.List;
import java.util.stream.Collectors;

@Path("/estadisticas/selecciones")
@Produces(MediaType.APPLICATION_JSON)
public class EstadisticaResource {

    @Inject
    private SeleccionRepository repository;

    @GET
    public List<EstadisticaSeleccionDTO> listar() {
        return repository.listarTodas().stream().map(s -> {
            EstadisticaSeleccionDTO dto = new EstadisticaSeleccionDTO();
            dto.setSeleccion(SeleccionResource.convertir(s));
            dto.setJugados(s.getPartidosJugados());
            dto.setGanados(s.getPartidosGanados());
            dto.setEmpatados(s.getPartidosEmpatados());
            dto.setPerdidos(s.getPartidosPerdidos());
            dto.setGolesFavor(s.getGolesFavor());
            dto.setGolesContra(s.getGolesContra());
            return dto;
        }).collect(Collectors.toList());
    }
}
