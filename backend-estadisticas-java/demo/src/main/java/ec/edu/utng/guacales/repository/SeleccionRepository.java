package ec.edu.utng.guacales.repository;

import ec.edu.utng.guacales.model.Seleccion;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import java.util.List;

@ApplicationScoped
public class SeleccionRepository {

    @PersistenceContext(unitName = "guacalesPU")
private EntityManager em;

public List<Seleccion> listarTodas() {
return em.createQuery(
"SELECT s FROM Seleccion s JOIN FETCH s.grupo ORDER BY s.nombre",
Seleccion.class)
                  .getResultList();
    }

public Seleccion buscarPorId(Long id) {
return em.createQuery(
"SELECT s FROM Seleccion s JOIN FETCH s.grupo WHERE s.id = :id",
Seleccion.class)
                .setParameter("id", id)
                .getResultStream()
                .findFirst()
                .orElse(null);
    }

public List<Seleccion> listarPorGrupo(String codigoGrupo) {
return em.createQuery(
"SELECT s FROM Seleccion s JOIN FETCH s.grupo WHERE s.grupo.codigo = :codigo ORDER BY s.puntos DESC, (s.golesFavor - s.golesContra) DESC",
Seleccion.class)
                .setParameter("codigo", codigoGrupo)
                .getResultList();
    }
}