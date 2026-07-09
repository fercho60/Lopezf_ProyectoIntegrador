-- =====================================================================
-- Actualizacion de resultados oficiales FIFA - UTN GolMundial 2026
-- Partidos 1-102: FINALIZADO con resultados reales
-- Partido 103 (Tercer puesto: Francia vs Inglaterra): PROGRAMADO
-- Partido 104 (Final: España vs Argentina): PROGRAMADO
-- =====================================================================
BEGIN;

UPDATE partido SET goleslocal=2, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=1 AND seleccion_visitante_id=3;
UPDATE partido SET goleslocal=2, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=2 AND seleccion_visitante_id=4;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=5 AND seleccion_visitante_id=6;
UPDATE partido SET goleslocal=4, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=13 AND seleccion_visitante_id=15;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=7 AND seleccion_visitante_id=8;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=9 AND seleccion_visitante_id=10;
UPDATE partido SET goleslocal=0, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=11 AND seleccion_visitante_id=12;
UPDATE partido SET goleslocal=2, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=14 AND seleccion_visitante_id=16;
UPDATE partido SET goleslocal=7, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=17 AND seleccion_visitante_id=20;
UPDATE partido SET goleslocal=1, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=19 AND seleccion_visitante_id=18;
UPDATE partido SET goleslocal=2, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=21 AND seleccion_visitante_id=22;
UPDATE partido SET goleslocal=5, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=24 AND seleccion_visitante_id=23;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=25 AND seleccion_visitante_id=27;
UPDATE partido SET goleslocal=2, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=26 AND seleccion_visitante_id=28;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=29 AND seleccion_visitante_id=32;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=31 AND seleccion_visitante_id=30;
UPDATE partido SET goleslocal=3, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=33 AND seleccion_visitante_id=34;
UPDATE partido SET goleslocal=1, golesvisitante=4, estado='FINALIZADO' WHERE seleccion_local_id=36 AND seleccion_visitante_id=35;
UPDATE partido SET goleslocal=3, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=37 AND seleccion_visitante_id=39;
UPDATE partido SET goleslocal=3, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=38 AND seleccion_visitante_id=40;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=41 AND seleccion_visitante_id=44;
UPDATE partido SET goleslocal=1, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=43 AND seleccion_visitante_id=42;
UPDATE partido SET goleslocal=4, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=45 AND seleccion_visitante_id=46;
UPDATE partido SET goleslocal=1, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=48 AND seleccion_visitante_id=47;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=4 AND seleccion_visitante_id=3;
UPDATE partido SET goleslocal=1, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=1 AND seleccion_visitante_id=2;
UPDATE partido SET goleslocal=4, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=8 AND seleccion_visitante_id=6;
UPDATE partido SET goleslocal=6, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=5 AND seleccion_visitante_id=7;
UPDATE partido SET goleslocal=0, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=12 AND seleccion_visitante_id=10;
UPDATE partido SET goleslocal=3, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=9 AND seleccion_visitante_id=11;
UPDATE partido SET goleslocal=2, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=13 AND seleccion_visitante_id=14;
UPDATE partido SET goleslocal=0, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=16 AND seleccion_visitante_id=15;
UPDATE partido SET goleslocal=2, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=17 AND seleccion_visitante_id=19;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=18 AND seleccion_visitante_id=20;
UPDATE partido SET goleslocal=5, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=21 AND seleccion_visitante_id=24;
UPDATE partido SET goleslocal=0, golesvisitante=4, estado='FINALIZADO' WHERE seleccion_local_id=23 AND seleccion_visitante_id=22;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=25 AND seleccion_visitante_id=26;
UPDATE partido SET goleslocal=1, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=28 AND seleccion_visitante_id=27;
UPDATE partido SET goleslocal=4, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=29 AND seleccion_visitante_id=31;
UPDATE partido SET goleslocal=2, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=30 AND seleccion_visitante_id=32;
UPDATE partido SET goleslocal=3, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=33 AND seleccion_visitante_id=36;
UPDATE partido SET goleslocal=3, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=35 AND seleccion_visitante_id=34;
UPDATE partido SET goleslocal=2, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=37 AND seleccion_visitante_id=38;
UPDATE partido SET goleslocal=1, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=40 AND seleccion_visitante_id=39;
UPDATE partido SET goleslocal=5, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=41 AND seleccion_visitante_id=43;
UPDATE partido SET goleslocal=1, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=42 AND seleccion_visitante_id=44;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=45 AND seleccion_visitante_id=48;
UPDATE partido SET goleslocal=0, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=47 AND seleccion_visitante_id=46;
UPDATE partido SET goleslocal=0, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=4 AND seleccion_visitante_id=1;
UPDATE partido SET goleslocal=1, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=3 AND seleccion_visitante_id=2;
UPDATE partido SET goleslocal=2, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=8 AND seleccion_visitante_id=5;
UPDATE partido SET goleslocal=3, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=6 AND seleccion_visitante_id=7;
UPDATE partido SET goleslocal=0, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=12 AND seleccion_visitante_id=9;
UPDATE partido SET goleslocal=4, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=10 AND seleccion_visitante_id=11;
UPDATE partido SET goleslocal=3, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=16 AND seleccion_visitante_id=13;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=15 AND seleccion_visitante_id=14;
UPDATE partido SET goleslocal=0, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=20 AND seleccion_visitante_id=19;
UPDATE partido SET goleslocal=2, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=18 AND seleccion_visitante_id=17;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=22 AND seleccion_visitante_id=24;
UPDATE partido SET goleslocal=1, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=23 AND seleccion_visitante_id=21;
UPDATE partido SET goleslocal=1, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=27 AND seleccion_visitante_id=26;
UPDATE partido SET goleslocal=1, golesvisitante=5, estado='FINALIZADO' WHERE seleccion_local_id=28 AND seleccion_visitante_id=25;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=32 AND seleccion_visitante_id=31;
UPDATE partido SET goleslocal=0, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=30 AND seleccion_visitante_id=29;
UPDATE partido SET goleslocal=1, golesvisitante=4, estado='FINALIZADO' WHERE seleccion_local_id=35 AND seleccion_visitante_id=33;
UPDATE partido SET goleslocal=5, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=34 AND seleccion_visitante_id=36;
UPDATE partido SET goleslocal=3, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=39 AND seleccion_visitante_id=38;
UPDATE partido SET goleslocal=1, golesvisitante=3, estado='FINALIZADO' WHERE seleccion_local_id=40 AND seleccion_visitante_id=37;
UPDATE partido SET goleslocal=0, golesvisitante=0, estado='FINALIZADO' WHERE seleccion_local_id=42 AND seleccion_visitante_id=41;
UPDATE partido SET goleslocal=3, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=44 AND seleccion_visitante_id=43;
UPDATE partido SET goleslocal=0, golesvisitante=2, estado='FINALIZADO' WHERE seleccion_local_id=47 AND seleccion_visitante_id=45;
UPDATE partido SET goleslocal=2, golesvisitante=1, estado='FINALIZADO' WHERE seleccion_local_id=46 AND seleccion_visitante_id=48;
-- Partidos de eliminatorias (nuevos, 73-104)
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (73, 3, 5, 7, NULL, '2026-06-28T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 0, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (76, 9, 22, 10, NULL, '2026-06-29T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (74, 17, 15, 11, NULL, '2026-06-29T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 1, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (75, 21, 10, 3, NULL, '2026-06-29T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 1, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (78, 19, 35, 8, NULL, '2026-06-30T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 1, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (77, 33, 24, 6, NULL, '2026-06-30T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 3, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (79, 1, 18, 1, NULL, '2026-06-30T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (80, 45, 44, 9, NULL, '2026-07-01T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (82, 25, 34, 16, NULL, '2026-07-01T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 3, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (81, 13, 6, 15, NULL, '2026-07-01T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (84, 29, 38, 7, NULL, '2026-07-02T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 3, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (83, 41, 46, 4, NULL, '2026-07-02T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (85, 8, 39, 5, NULL, '2026-07-02T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 2, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (88, 14, 27, 8, NULL, '2026-07-03T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 1, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (86, 37, 32, 13, NULL, '2026-07-03T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 3, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (87, 42, 48, 14, NULL, '2026-07-03T18:00:00', 'ROUND_OF_32', 'FINALIZADO', 1, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (90, 5, 10, 10, NULL, '2026-07-04T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 0, 3);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (89, 15, 33, 12, NULL, '2026-07-04T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 0, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (91, 9, 35, 6, NULL, '2026-07-05T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 1, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (92, 1, 45, 1, NULL, '2026-07-05T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 2, 3);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (93, 41, 29, 8, NULL, '2026-07-06T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 0, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (94, 13, 25, 16, NULL, '2026-07-06T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 1, 4);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (95, 37, 27, 9, NULL, '2026-07-07T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 3, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (96, 8, 42, 5, NULL, '2026-07-07T18:00:00', 'ROUND_OF_16', 'FINALIZADO', 0, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (97, 33, 10, 11, NULL, '2026-07-09T18:00:00', 'QUARTERFINALS', 'FINALIZADO', 2, 0);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (98, 29, 25, 7, NULL, '2026-07-10T18:00:00', 'QUARTERFINALS', 'FINALIZADO', 2, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (99, 35, 45, 13, NULL, '2026-07-11T18:00:00', 'QUARTERFINALS', 'FINALIZADO', 1, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (100, 37, 8, 14, NULL, '2026-07-11T18:00:00', 'QUARTERFINALS', 'FINALIZADO', 3, 1);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (101, 33, 29, 8, NULL, '2026-07-14T18:00:00', 'SEMIFINALS', 'FINALIZADO', 0, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (102, 45, 37, 9, NULL, '2026-07-15T18:00:00', 'SEMIFINALS', 'FINALIZADO', 1, 2);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (103, 33, 45, 13, NULL, '2026-07-18T18:00:00', 'TERCER_PUESTO', 'PROGRAMADO', NULL, NULL);
INSERT INTO partido (numero_partido_fifa, seleccion_local_id, seleccion_visitante_id, sede_id, grupo_id, fecha_partido, fase, estado, goleslocal, golesvisitante) VALUES (104, 29, 37, 6, NULL, '2026-07-19T18:00:00', 'FINAL', 'PROGRAMADO', NULL, NULL);
-- Recalcular estadisticas de las 48 selecciones (solo aplica a fase de grupos)
UPDATE seleccion SET puntos = 0, partidosjugados = 0, partidosganados = 0,
  partidosempatados = 0, partidosperdidos = 0, golesfavor = 0, golescontra = 0;

UPDATE seleccion s SET
  partidosjugados = sub.jugados,
  partidosganados = sub.ganados,
  partidosempatados = sub.empatados,
  partidosperdidos = sub.perdidos,
  golesfavor = sub.gf,
  golescontra = sub.gc,
  puntos = sub.pts
FROM (
  SELECT equipo_id,
    COUNT(*) AS jugados,
    SUM(CASE WHEN gf > gc THEN 1 ELSE 0 END) AS ganados,
    SUM(CASE WHEN gf = gc THEN 1 ELSE 0 END) AS empatados,
    SUM(CASE WHEN gf < gc THEN 1 ELSE 0 END) AS perdidos,
    SUM(gf) AS gf,
    SUM(gc) AS gc,
    SUM(CASE WHEN gf > gc THEN 3 WHEN gf = gc THEN 1 ELSE 0 END) AS pts
  FROM (
    SELECT seleccion_local_id AS equipo_id, goleslocal AS gf, golesvisitante AS gc
    FROM partido WHERE fase = 'FASE_DE_GRUPOS' AND estado = 'FINALIZADO'
    UNION ALL
    SELECT seleccion_visitante_id AS equipo_id, golesvisitante AS gf, goleslocal AS gc
    FROM partido WHERE fase = 'FASE_DE_GRUPOS' AND estado = 'FINALIZADO'
  ) t
  GROUP BY equipo_id
) sub
WHERE s.id = sub.equipo_id;

COMMIT;
