package ec.edu.utng.guacales.repository;

import ec.edu.utng.guacales.model.Sede;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import java.util.List;

@ApplicationScoped
public class SedeRepository {

    @PersistenceContext(unitName = "guacalesPU")
    private EntityManager em;

    public List<Sede> listarTodas() {
        return em.createQuery("SELECT s FROM Sede s ORDER BY s.nombre", Sede.class)
                .getResultList();
    }

    public Sede buscarPorId(Long id) {
        return em.find(Sede.class, id);
    }
}
