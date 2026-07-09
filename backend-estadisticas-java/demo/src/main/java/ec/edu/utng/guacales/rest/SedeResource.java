package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.SedeDTO;
import ec.edu.utng.guacales.model.Sede;
import ec.edu.utng.guacales.repository.SedeRepository;
import jakarta.inject.Inject;
import jakarta.ws.rs.GET;
import jakarta.ws.rs.Path;
import jakarta.ws.rs.PathParam;
import jakarta.ws.rs.Produces;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import java.util.List;
import java.util.stream.Collectors;

@Path("/sedes")
@Produces(MediaType.APPLICATION_JSON)
public class SedeResource {

    @Inject
    private SedeRepository repository;

    @GET
    public List<SedeDTO> listar() {
        return repository.listarTodas().stream()
                .map(this::convertir)
                .collect(Collectors.toList());
    }

    @GET
    @Path("/{id}")
    public Response buscarPorId(@PathParam("id") Long id) {
        Sede s = repository.buscarPorId(id);
        if (s == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("{\"codigo\":\"SEDE_NO_ENCONTRADA\",\"mensaje\":\"Sede no encontrada\"}")
                    .build();
        }
        return Response.ok(convertir(s)).build();
    }

    private SedeDTO convertir(Sede s) {
        SedeDTO dto = new SedeDTO();
        dto.setId(s.getId());
        dto.setNombre(s.getNombre());
        dto.setCiudad(s.getCiudad());
        dto.setPais(s.getPais());
        dto.setCapacidad(s.getCapacidad());
        return dto;
    }
}
