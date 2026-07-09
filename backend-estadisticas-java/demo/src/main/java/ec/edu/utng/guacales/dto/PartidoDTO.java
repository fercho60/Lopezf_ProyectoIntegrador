package ec.edu.utng.guacales.dto;

public class PartidoDTO {
    private Long id;
    private SeleccionDTO seleccionLocal;
    private SeleccionDTO seleccionVisitante;
    private String fechaHora;
    private String sede;
    private String fase;
    private String grupo;
    private String estado;
    private Integer golesLocal;
    private Integer golesVisitante;
    private CuotasPartidoDTO cuotas;

    public PartidoDTO() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public SeleccionDTO getSeleccionLocal() { return seleccionLocal; }
    public void setSeleccionLocal(SeleccionDTO seleccionLocal) { this.seleccionLocal = seleccionLocal; }
    public SeleccionDTO getSeleccionVisitante() { return seleccionVisitante; }
    public void setSeleccionVisitante(SeleccionDTO seleccionVisitante) { this.seleccionVisitante = seleccionVisitante; }
    public String getFechaHora() { return fechaHora; }
    public void setFechaHora(String fechaHora) { this.fechaHora = fechaHora; }
    public String getSede() { return sede; }
    public void setSede(String sede) { this.sede = sede; }
    public String getFase() { return fase; }
    public void setFase(String fase) { this.fase = fase; }
    public String getGrupo() { return grupo; }
    public void setGrupo(String grupo) { this.grupo = grupo; }
    public String getEstado() { return estado; }
    public void setEstado(String estado) { this.estado = estado; }
    public Integer getGolesLocal() { return golesLocal; }
    public void setGolesLocal(Integer golesLocal) { this.golesLocal = golesLocal; }
    public Integer getGolesVisitante() { return golesVisitante; }
    public void setGolesVisitante(Integer golesVisitante) { this.golesVisitante = golesVisitante; }
    public CuotasPartidoDTO getCuotas() { return cuotas; }
    public void setCuotas(CuotasPartidoDTO cuotas) { this.cuotas = cuotas; }
}
