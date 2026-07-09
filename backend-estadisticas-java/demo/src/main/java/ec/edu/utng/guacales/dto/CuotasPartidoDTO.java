package ec.edu.utng.guacales.dto;

public class CuotasPartidoDTO {
    private Double local;
    private Double empate;
    private Double visitante;

    public CuotasPartidoDTO() {}
    public CuotasPartidoDTO(Double local, Double empate, Double visitante) {
        this.local = local;
        this.empate = empate;
        this.visitante = visitante;
    }

    public Double getLocal() { return local; }
    public void setLocal(Double local) { this.local = local; }
    public Double getEmpate() { return empate; }
    public void setEmpate(Double empate) { this.empate = empate; }
    public Double getVisitante() { return visitante; }
    public void setVisitante(Double visitante) { this.visitante = visitante; }
}
