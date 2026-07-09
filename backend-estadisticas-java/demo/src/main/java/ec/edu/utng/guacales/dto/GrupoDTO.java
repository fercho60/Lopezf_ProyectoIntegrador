package ec.edu.utng.guacales.dto;

import java.util.List;

public class GrupoDTO {
    private Long id;
    private String nombre;
    private List<PosicionEquipoDTO> posiciones;

    public GrupoDTO() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }
    public List<PosicionEquipoDTO> getPosiciones() { return posiciones; }
    public void setPosiciones(List<PosicionEquipoDTO> posiciones) { this.posiciones = posiciones; }
}
