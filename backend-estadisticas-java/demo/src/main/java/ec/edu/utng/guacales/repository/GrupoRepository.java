package ec.edu.utng.guacales.repository;

import ec.edu.utng.guacales.model.Grupo;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.persistence.EntityManager;
import jakarta.persistence.PersistenceContext;
import java.util.List;

@ApplicationScoped
public class GrupoRepository {

    @PersistenceContext(unitName = "guacalesPU")
    private EntityManager em;

    public List<Grupo> listarTodos() {
        return em.createQuery(
                "SELECT DISTINCT g FROM Grupo g LEFT JOIN FETCH g.selecciones ORDER BY g.codigo",
                Grupo.class)
                .getResultList();
    }

    public Grupo buscarPorCodigo(String codigo) {
        return em.createQuery(
                "SELECT DISTINCT g FROM Grupo g LEFT JOIN FETCH g.selecciones WHERE g.codigo = :codigo",
                Grupo.class)
                .setParameter("codigo", codigo)
                .getResultStream()
                .findFirst()
                .orElse(null);
    }
}
