package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.SeleccionDTO;
import ec.edu.utng.guacales.model.Seleccion;
import ec.edu.utng.guacales.repository.SeleccionRepository;
import jakarta.inject.Inject;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import java.util.List;
import java.util.stream.Collectors;

@Path("/selecciones")
@Produces(MediaType.APPLICATION_JSON)
public class SeleccionResource {

    @Inject
    private SeleccionRepository repository;

    @GET
    public List<SeleccionDTO> listar() {
        return repository.listarTodas().stream()
                .map(SeleccionResource::convertir)
                .collect(Collectors.toList());
    }

    public static SeleccionDTO convertir(Seleccion s) {
        SeleccionDTO dto = new SeleccionDTO();
        dto.setId(s.getId());
        dto.setNombre(s.getNombre());
        dto.setCodigoPais(s.getCodigoFifa());
        dto.setGrupo(s.getGrupo() != null ? s.getGrupo().getNombre() : null);
        return dto;
    }
}
