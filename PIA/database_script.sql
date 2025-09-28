-- Script para crear la base de datos del Sistema de Punto de Venta
-- Base de datos: pos_system

CREATE DATABASE IF NOT EXISTS pos_system;
USE pos_system;

-- Tabla de usuarios
CREATE TABLE IF NOT EXISTS users (
    id_user INT PRIMARY KEY AUTO_INCREMENT,
    name_user VARCHAR(50) NOT NULL,
    lastName_user VARCHAR(50) NOT NULL,
    email_user VARCHAR(100) UNIQUE NOT NULL,
    password_user VARCHAR(255) NOT NULL,
    user_type ENUM('admin', 'employee') DEFAULT 'employee',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabla de productos (mejorada)
CREATE TABLE IF NOT EXISTS product (
    id_product INT PRIMARY KEY,
    name_product VARCHAR(100) NOT NULL,
    categoria_product VARCHAR(50) NOT NULL,
    precioCompra_product DECIMAL(10,2) NOT NULL,
    precioVenta_product DECIMAL(10,2) NOT NULL,
    stock_product INT DEFAULT 0,
    stock_min INT DEFAULT 5,
    description_product TEXT,
    status_product ENUM('active', 'inactive') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Tabla de clientes
CREATE TABLE IF NOT EXISTS customers (
    id_customer INT PRIMARY KEY AUTO_INCREMENT,
    name_customer VARCHAR(100) NOT NULL,
    phone_customer VARCHAR(15),
    email_customer VARCHAR(100),
    address_customer TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla de ventas
CREATE TABLE IF NOT EXISTS sales (
    id_sale INT PRIMARY KEY AUTO_INCREMENT,
    id_user INT NOT NULL,
    id_customer INT NULL,
    total_sale DECIMAL(10,2) NOT NULL,
    payment_method ENUM('cash', 'card', 'transfer') DEFAULT 'cash',
    sale_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    status_sale ENUM('completed', 'cancelled') DEFAULT 'completed',
    FOREIGN KEY (id_user) REFERENCES users(id_user),
    FOREIGN KEY (id_customer) REFERENCES customers(id_customer)
);

-- Tabla de detalles de venta
CREATE TABLE IF NOT EXISTS sale_details (
    id_detail INT PRIMARY KEY AUTO_INCREMENT,
    id_sale INT NOT NULL,
    id_product INT NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL(10,2) NOT NULL,
    total_price DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (id_sale) REFERENCES sales(id_sale) ON DELETE CASCADE,
    FOREIGN KEY (id_product) REFERENCES product(id_product)
);

-- Tabla de movimientos de inventario
CREATE TABLE IF NOT EXISTS inventory_movements (
    id_movement INT PRIMARY KEY AUTO_INCREMENT,
    id_product INT NOT NULL,
    movement_type ENUM('in', 'out', 'adjustment') NOT NULL,
    quantity INT NOT NULL,
    reason VARCHAR(100),
    id_user INT NOT NULL,
    movement_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (id_product) REFERENCES product(id_product),
    FOREIGN KEY (id_user) REFERENCES users(id_user)
);

-- Insertar usuario administrador por defecto
INSERT INTO users (name_user, lastName_user, email_user, password_user, user_type) 
VALUES ('ADMIN', 'SISTEMA', 'admin@pos.com', 'admin123', 'admin');

-- Insertar cliente por defecto para ventas sin cliente específico
INSERT INTO customers (name_customer, phone_customer, email_customer) 
VALUES ('Cliente General', '0000000000', 'general@pos.com');

-- Insertar algunos productos de ejemplo
INSERT INTO product (id_product, name_product, categoria_product, precioCompra_product, precioVenta_product, stock_product, description_product) VALUES
(1, 'Coca Cola', 'Bebidas', 15.00, 25.00, 50, 'Refresco de cola 600ml'),
(2, 'Pan Blanco', 'Panaderia', 20.00, 35.00, 30, 'Pan blanco integral'),
(3, 'Leche Entera', 'Lacteos', 18.00, 28.00, 25, 'Leche entera 1 litro'),
(4, 'Arroz', 'Abarrotes', 45.00, 65.00, 40, 'Arroz blanco 1kg'),
(5, 'Aceite Vegetal', 'Abarrotes', 35.00, 55.00, 20, 'Aceite vegetal 1 litro');