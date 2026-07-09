package ec.edu.utng.guacales.dto;

public class RespuestaSesionDTO {
    private String token;
    private UsuarioDTO usuario;

    public RespuestaSesionDTO() {}
    public RespuestaSesionDTO(String token, UsuarioDTO usuario) {
        this.token = token;
        this.usuario = usuario;
    }

    public String getToken() { return token; }
    public void setToken(String token) { this.token = token; }
    public UsuarioDTO getUsuario() { return usuario; }
    public void setUsuario(UsuarioDTO usuario) { this.usuario = usuario; }
}
