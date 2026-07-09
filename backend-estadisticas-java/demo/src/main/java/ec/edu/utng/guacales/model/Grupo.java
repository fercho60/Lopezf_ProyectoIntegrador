package ec.edu.utng.guacales.model;

import jakarta.persistence.*;
import java.util.List;

@Entity
@Table(name = "grupo")
public class Grupo {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false, length = 100)
    private String nombre;

    @Column(nullable = false, unique = true, length = 5)
    private String codigo; // A, B, C ... L

    @OneToMany(mappedBy = "grupo")
    private List<Seleccion> selecciones;

    public Grupo() {}

    public Long getId() { return id; }
    public void setId(Long id) { this.id = id; }
    public String getNombre() { return nombre; }
    public void setNombre(String nombre) { this.nombre = nombre; }
    public String getCodigo() { return codigo; }
    public void setCodigo(String codigo) { this.codigo = codigo; }
    public List<Seleccion> getSelecciones() { return selecciones; }
    public void setSelecciones(List<Seleccion> selecciones) { this.selecciones = selecciones; }
}
