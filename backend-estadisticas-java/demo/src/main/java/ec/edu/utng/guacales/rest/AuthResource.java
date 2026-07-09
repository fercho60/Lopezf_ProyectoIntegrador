package ec.edu.utng.guacales.rest;

import ec.edu.utng.guacales.dto.*;
import ec.edu.utng.guacales.model.Usuario;
import ec.edu.utng.guacales.service.AuthService;
import jakarta.inject.Inject;
import jakarta.ws.rs.*;
import jakarta.ws.rs.core.MediaType;
import jakarta.ws.rs.core.Response;

@Path("/autenticacion")
@Produces(MediaType.APPLICATION_JSON)
@Consumes(MediaType.APPLICATION_JSON)
public class AuthResource {

    @Inject
    private AuthService authService;

    @POST
    @Path("/registro")
    public Response registrar(SolicitudRegistroDTO solicitud) {
        if (solicitud.getNombre() == null || solicitud.getNombre().isBlank()
                || solicitud.getCorreo() == null || solicitud.getCorreo().isBlank()
                || solicitud.getContrasena() == null || solicitud.getContrasena().length() < 8) {
            return Response.status(422)
                    .entity(new ErrorDTO("DATOS_INVALIDOS", "Nombre, correo y contrasena (min 8 caracteres) son requeridos"))
                    .build();
        }

        AuthService.ResultadoAuth resultado = authService.registrar(
                solicitud.getNombre(), solicitud.getCorreo(), solicitud.getContrasena());

        if (resultado.error != null) {
            return Response.status(Response.Status.CONFLICT)
                    .entity(new ErrorDTO("CORREO_DUPLICADO", "El correo ya esta registrado"))
                    .build();
        }

        return Response.status(Response.Status.CREATED)
                .entity(construirRespuestaSesion(resultado.usuario))
                .build();
    }

    @POST
    @Path("/sesion")
    public Response iniciarSesion(SolicitudSesionDTO solicitud) {
        AuthService.ResultadoAuth resultado = authService.iniciarSesion(
                solicitud.getCorreo(), solicitud.getContrasena());

        if (resultado.error != null) {
            return Response.status(Response.Status.UNAUTHORIZED)
                    .entity(new ErrorDTO("CREDENCIALES_INVALIDAS", "Correo o contrasena incorrectos"))
                    .build();
        }

        return Response.ok(construirRespuestaSesion(resultado.usuario)).build();
    }

    private RespuestaSesionDTO construirRespuestaSesion(Usuario u) {
        UsuarioDTO usuarioDTO = new UsuarioDTO();
        usuarioDTO.setId(u.getId());
        usuarioDTO.setNombre(u.getNombre());
        usuarioDTO.setCorreo(u.getEmail());
        usuarioDTO.setRol(u.getRol().getNombre().toLowerCase());

        String token = authService.generarToken(u);
        return new RespuestaSesionDTO(token, usuarioDTO);
    }
}
