-- ============================================
-- POS tiendita - esquema inicial
-- MySQL 8.x, utf8mb4
-- ============================================

CREATE DATABASE IF NOT EXISTS pos_tiendita
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;

USE pos_tiendita;

-- --------------------------------------------
-- proveedores
-- --------------------------------------------
CREATE TABLE proveedores (
  id BIGINT NOT NULL AUTO_INCREMENT,
  nombre VARCHAR(200) NOT NULL,
  telefono VARCHAR(40) NULL,
  notas VARCHAR(500) NULL,
  activo TINYINT(1) NOT NULL DEFAULT 1,
  creado_en DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (id),
  KEY ix_proveedores_activo (activo),
  KEY ix_proveedores_nombre (nombre)
) ENGINE=InnoDB;

-- --------------------------------------------
-- productos
-- --------------------------------------------
CREATE TABLE productos (
  id BIGINT NOT NULL AUTO_INCREMENT,
  id_proveedor BIGINT NULL,
  nombre VARCHAR(300) NOT NULL,
  codigo_barras VARCHAR(64) NULL,
  precio_venta DECIMAL(12,4) NOT NULL,
  precio_costo DECIMAL(12,4) NULL COMMENT 'Costo al proveedor por unidad (último conocido)',
  existencias INT NOT NULL DEFAULT 0,
  activo TINYINT(1) NOT NULL DEFAULT 1,
  creado_en DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  PRIMARY KEY (id),
  UNIQUE KEY uk_productos_codigo_barras (codigo_barras),
  KEY ix_productos_proveedor (id_proveedor),
  KEY ix_productos_activo_nombre (activo, nombre),
  CONSTRAINT fk_productos_proveedor
    FOREIGN KEY (id_proveedor) REFERENCES proveedores (id)
    ON DELETE SET NULL
    ON UPDATE CASCADE
) ENGINE=InnoDB;

-- --------------------------------------------
-- ventas (cabecera de ticket)
-- id_usuario: reservado para login futuro (sin FK hasta exista usuarios)
-- --------------------------------------------
CREATE TABLE ventas (
  id BIGINT NOT NULL AUTO_INCREMENT,
  fecha_hora DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
  total DECIMAL(12,4) NOT NULL,
  forma_pago VARCHAR(20) NULL COMMENT 'ej. efectivo, tarjeta, mixto',
  id_usuario BIGINT NULL,
  PRIMARY KEY (id),
  KEY ix_ventas_fecha (fecha_hora),
  KEY ix_ventas_usuario (id_usuario)
) ENGINE=InnoDB;

-- --------------------------------------------
-- lineas_venta (detalle; precio y costo congelados al vender)
-- --------------------------------------------
CREATE TABLE lineas_venta (
  id BIGINT NOT NULL AUTO_INCREMENT,
  id_venta BIGINT NOT NULL,
  id_producto BIGINT NOT NULL,
  cantidad INT NOT NULL,
  precio_unitario DECIMAL(12,4) NOT NULL,
  costo_unitario DECIMAL(12,4) NULL COMMENT 'Snapshot de precio_costo al momento de la venta',
  importe_linea DECIMAL(12,4) NOT NULL,
  PRIMARY KEY (id),
  KEY ix_lineas_venta (id_venta),
  KEY ix_lineas_producto (id_producto),
  CONSTRAINT fk_lineas_venta
    FOREIGN KEY (id_venta) REFERENCES ventas (id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE,
  CONSTRAINT fk_lineas_producto
    FOREIGN KEY (id_producto) REFERENCES productos (id)
    ON DELETE RESTRICT
    ON UPDATE CASCADE
) ENGINE=InnoDB;