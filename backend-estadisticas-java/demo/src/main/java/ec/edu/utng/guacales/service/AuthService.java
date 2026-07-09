package ec.edu.utng.guacales.service;

import ec.edu.utng.guacales.model.Rol;
import ec.edu.utng.guacales.model.Usuario;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.security.Keys;
import jakarta.enterprise.context.ApplicationScoped;
import jakarta.persistence.EntityManager;
import jakarta.persistence.NoResultException;
import jakarta.persistence.PersistenceContext;
import jakarta.transaction.Transactional;
import org.mindrot.jbcrypt.BCrypt;

import javax.crypto.SecretKey;
import java.util.Date;

@ApplicationScoped
public class AuthService {

    @PersistenceContext(unitName = "guacalesPU")
    private EntityManager em;

    // Clave fija de 32+ caracteres solo para el entorno de desarrollo del proyecto.
    private static final SecretKey CLAVE_JWT =
            Keys.hmacShaKeyFor("guacales-utn-golmundial-2026-clave-secreta-dev".getBytes());

    private static final long DURACION_TOKEN_MS = 1000L * 60 * 60 * 24; // 24 horas

    public static class ResultadoAuth {
        public Usuario usuario;
        public String error; // null si fue exitoso
    }

    @Transactional
    public ResultadoAuth registrar(String nombre, String correo, String contrasena) {
        ResultadoAuth resultado = new ResultadoAuth();

        Long existentes = em.createQuery(
                "SELECT COUNT(u) FROM Usuario u WHERE u.email = :correo", Long.class)
                .setParameter("correo", correo)
                .getSingleResult();

        if (existentes > 0) {
            resultado.error = "CORREO_DUPLICADO";
            return resultado;
        }

        Rol rolUsuario = em.createQuery(
                "SELECT r FROM Rol r WHERE r.nombre = 'USUARIO'", Rol.class)
                .getSingleResult();

        Usuario u = new Usuario();
        u.setNombre(nombre);
        u.setEmail(correo);
        u.setUsername(correo);
        u.setPassword(BCrypt.hashpw(contrasena, BCrypt.gensalt()));
        u.setRol(rolUsuario);

        em.persist(u);
        em.flush();

        // TODO (integracion con UTNGolCoin): aqui se debe notificar al Servicio UTNGolCoin
        // para crear la billetera del usuario y acreditar el bono de bienvenida de 10 UTNGolCoin.
        // Ejemplo futuro: llamar via HTTP/REST al endpoint de UTNGolCoin con el id del usuario creado.

        resultado.usuario = u;
        return resultado;
    }

    public ResultadoAuth iniciarSesion(String correo, String contrasena) {
        ResultadoAuth resultado = new ResultadoAuth();
        try {
            Usuario u = em.createQuery(
                    "SELECT u FROM Usuario u JOIN FETCH u.rol WHERE u.email = :correo", Usuario.class)
                    .setParameter("correo", correo)
                    .getSingleResult();

            if (!BCrypt.checkpw(contrasena, u.getPassword())) {
                resultado.error = "CREDENCIALES_INVALIDAS";
                return resultado;
            }

            resultado.usuario = u;
            return resultado;
        } catch (NoResultException e) {
            resultado.error = "CREDENCIALES_INVALIDAS";
            return resultado;
        }
    }

    public String generarToken(Usuario u) {
        return Jwts.builder()
                .subject(String.valueOf(u.getId()))
                .claim("correo", u.getEmail())
                .claim("rol", u.getRol().getNombre())
                .issuedAt(new Date())
                .expiration(new Date(System.currentTimeMillis() + DURACION_TOKEN_MS))
                .signWith(CLAVE_JWT)
                .compact();
    }
}
