package ec.edu.utng.guacales.model;

import jakarta.persistence.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "partido")
public class Partido {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(name = "numero_partido_fifa")
    private Integer numeroPartidoFifa;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "seleccion_local_id", nullable = false)
    private Seleccion seleccionLocal;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "seleccion_visitante_id", nullable = false)
    private Seleccion seleccionVisitante;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "sede_id", nullable = false)
    private Sede sede;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "grupo_id")
    private Grupo grupo;

    @Column(name = "fecha_partido", nullable = false)
    private LocalDateTime fechaPartido;

    @Column(length = 30)
    private String fase;

    private Integer golesLocal;
    private Integer golesVisitante;

    @Column(length = 20)
    private String estado = "PROGRAMADO";

    @Column(name = "cuota_local")
    private Double cuotaLocal;

    @Column(name = "cuota_empate")
    private Double cuotaEmpate;

    @Column(name = "cuota_visitante")
    private Double cuotaVisitante;

    public Partido() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public Integer getNumeroPartidoFifa() { return numeroPartidoFifa; }
    public void setNumeroPartidoFifa(Integer numeroPartidoFifa) { this.numeroPartidoFifa = numeroPartidoFifa; }
    public Seleccion getSeleccionLocal() { return seleccionLocal; }
    public void setSeleccionLocal(Seleccion seleccionLocal) { this.seleccionLocal = seleccionLocal; }
    public Seleccion getSeleccionVisitante() { return seleccionVisitante; }
    public void setSeleccionVisitante(Seleccion seleccionVisitante) { this.seleccionVisitante = seleccionVisitante; }
    public Sede getSede() { return sede; }
    public void setSede(Sede sede) { this.sede = sede; }
    public Grupo getGrupo() { return grupo; }
    public void setGrupo(Grupo grupo) { this.grupo = grupo; }
    public LocalDateTime getFechaPartido() { return fechaPartido; }
    public void setFechaPartido(LocalDateTime fechaPartido) { this.fechaPartido = fechaPartido; }
    public String getFase() { return fase; }
    public void setFase(String fase) { this.fase = fase; }
    public Integer getGolesLocal() { return golesLocal; }
    public void setGolesLocal(Integer golesLocal) { this.golesLocal = golesLocal; }
    public Integer getGolesVisitante() { return golesVisitante; }
    public void setGolesVisitante(Integer golesVisitante) { this.golesVisitante = golesVisitante; }
    public String getEstado() { return estado; }
    public void setEstado(String estado) { this.estado = estado; }
    public Double getCuotaLocal() { return cuotaLocal; }
    public void setCuotaLocal(Double cuotaLocal) { this.cuotaLocal = cuotaLocal; }
    public Double getCuotaEmpate() { return cuotaEmpate; }
    public void setCuotaEmpate(Double cuotaEmpate) { this.cuotaEmpate = cuotaEmpate; }
    public Double getCuotaVisitante() { return cuotaVisitante; }
    public void setCuotaVisitante(Double cuotaVisitante) { this.cuotaVisitante = cuotaVisitante; }
}
