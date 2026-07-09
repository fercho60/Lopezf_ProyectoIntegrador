package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.CuotasPartidoDTO;
import ec.edu.utng.guacales.dto.PartidoDTO;
import ec.edu.utng.guacales.dto.ResultadoDTO;
import ec.edu.utng.guacales.model.Partido;
import ec.edu.utng.guacales.repository.PartidoRepository;
import ec.edu.utng.guacales.service.ResultadoService;
import jakarta.inject.Inject;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;
import java.time.LocalDate;
import java.time.format.DateTimeFormatter;
import java.util.List;
import java.util.stream.Collectors;

@Path("/partidos")
@Produces(MediaType.APPLICATION_JSON)
public class PartidoResource {

    @Inject
    private PartidoRepository repository;

    @Inject
    private ResultadoService resultadoService;

    @GET
    public List<PartidoDTO> listar(
            @QueryParam("estado") String estado,
            @QueryParam("grupo") String grupo,
            @QueryParam("fase") String fase,
            @QueryParam("fecha") String fechaStr) {

        LocalDate fecha = (fechaStr != null && !fechaStr.isBlank()) ? LocalDate.parse(fechaStr) : null;

        return repository.listar(estado, grupo, fase, fecha).stream()
                .map(this::convertir)
                .collect(Collectors.toList());
    }

    @GET
    @Path("/{partidoId}")
    public Response buscarPorId(@PathParam("partidoId") Long partidoId) {
        Partido p = repository.buscarPorId(partidoId);
        if (p == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("{\"codigo\":\"PARTIDO_NO_ENCONTRADO\",\"mensaje\":\"Partido no encontrado\"}")
                    .build();
        }
        return Response.ok(convertir(p)).build();
    }

    @PUT
    @Path("/{partidoId}/resultado")
    @Consumes(MediaType.APPLICATION_JSON)
    public Response registrarResultado(@PathParam("partidoId") Long partidoId, ResultadoDTO resultado) {
        if (resultado.getGolesLocal() == null || resultado.getGolesVisitante() == null
                || resultado.getGolesLocal() < 0 || resultado.getGolesVisitante() < 0) {
            return Response.status(422)
                    .entity("{\"codigo\":\"RESULTADO_INVALIDO\",\"mensaje\":\"Los goles deben ser numeros positivos\"}")
                    .build();
        }

        Partido guardado = resultadoService.registrarResultado(
                partidoId, resultado.getGolesLocal(), resultado.getGolesVisitante());

        if (guardado == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("{\"codigo\":\"PARTIDO_NO_ENCONTRADO\",\"mensaje\":\"Partido no encontrado\"}")
                    .build();
        }

        Partido actualizado = repository.buscarPorId(partidoId);

        if (actualizado == null) {
            return Response.status(Response.Status.NOT_FOUND)
                    .entity("{\"codigo\":\"PARTIDO_NO_ENCONTRADO\",\"mensaje\":\"Partido no encontrado\"}")
                    .build();
        }

        return Response.ok(convertir(actualizado)).build();
    }

    private PartidoDTO convertir(Partido p) {
        PartidoDTO dto = new PartidoDTO();        dto.setId(p.getId());
        dto.setSeleccionLocal(SeleccionResource.convertir(p.getSeleccionLocal()));
        dto.setSeleccionVisitante(SeleccionResource.convertir(p.getSeleccionVisitante()));
        dto.setFechaHora(p.getFechaPartido().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME) + "Z");
        dto.setSede(p.getSede().getNombre() + ", " + p.getSede().getCiudad());
        dto.setFase(p.getFase());
        dto.setGrupo(p.getGrupo() != null ? p.getGrupo().getNombre() : null);
        dto.setEstado(p.getEstado());
        dto.setGolesLocal(p.getGolesLocal());
        dto.setGolesVisitante(p.getGolesVisitante());
        dto.setCuotas(new CuotasPartidoDTO(p.getCuotaLocal(), p.getCuotaEmpate(), p.getCuotaVisitante()));
        return dto;
    }
}
