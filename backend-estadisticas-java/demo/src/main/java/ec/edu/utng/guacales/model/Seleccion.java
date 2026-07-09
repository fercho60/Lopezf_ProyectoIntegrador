package ec.edu.utng.guacales.model;

import jakarta.persistence.*;

@Entity
@Table(name = "seleccion")
public class Seleccion {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 100)
    private String nombre;

    @Column(name = "codigo_fifa", length = 5)
    private String codigoFifa;

    @Column(length = 100)
    private String confederacion;

    @Column(name = "es_anfitrion")
    private Boolean esAnfitrion = false;

    @Column(length = 100)
    private String clasificacion;

    @Column(name = "bandera_url")
    private String banderaUrl;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "grupo_id")
    private Grupo grupo;

    private int puntos = 0;
    private int partidosJugados = 0;
    private int partidosGanados = 0;
    private int partidosEmpatados = 0;
    private int partidosPerdidos = 0;
    private int golesFavor = 0;
    private int golesContra = 0;

    public Seleccion() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }
    public String getCodigoFifa() { return codigoFifa; }
    public void setCodigoFifa(String codigoFifa) { this.codigoFifa = codigoFifa; }
    public String getConfederacion() { return confederacion; }
    public void setConfederacion(String confederacion) { this.confederacion = confederacion; }
    public Boolean getEsAnfitrion() { return esAnfitrion; }
    public void setEsAnfitrion(Boolean esAnfitrion) { this.esAnfitrion = esAnfitrion; }
    public String getClasificacion() { return clasificacion; }
    public void setClasificacion(String clasificacion) { this.clasificacion = clasificacion; }
    public String getBanderaUrl() { return banderaUrl; }
    public void setBanderaUrl(String banderaUrl) { this.banderaUrl = banderaUrl; }
    public Grupo getGrupo() { return grupo; }
    public void setGrupo(Grupo grupo) { this.grupo = grupo; }
    public int getPuntos() { return puntos; }
    public void setPuntos(int puntos) { this.puntos = puntos; }
    public int getPartidosJugados() { return partidosJugados; }
    public void setPartidosJugados(int partidosJugados) { this.partidosJugados = partidosJugados; }
    public int getPartidosGanados() { return partidosGanados; }
    public void setPartidosGanados(int partidosGanados) { this.partidosGanados = partidosGanados; }
    public int getPartidosEmpatados() { return partidosEmpatados; }
    public void setPartidosEmpatados(int partidosEmpatados) { this.partidosEmpatados = partidosEmpatados; }
    public int getPartidosPerdidos() { return partidosPerdidos; }
    public void setPartidosPerdidos(int partidosPerdidos) { this.partidosPerdidos = partidosPerdidos; }
    public int getGolesFavor() { return golesFavor; }
    public void setGolesFavor(int golesFavor) { this.golesFavor = golesFavor; }
    public int getGolesContra() { return golesContra; }
    public void setGolesContra(int golesContra) { this.golesContra = golesContra; }
    public int getDiferenciaGoles() { return golesFavor - golesContra; }
}
