package ec.edu.utng.guacales.dto;

public class SolicitudSesionDTO {
    private String correo;
    private String contrasena;

    public SolicitudSesionDTO() {}

    public String getCorreo() { return correo; }
    public void setCorreo(String correo) { this.correo = correo; }
    public String getContrasena() { return contrasena; }
    public void setContrasena(String contrasena) { this.contrasena = contrasena; }
}
