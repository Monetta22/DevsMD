CREATE DATABASE metodologias;
USE metodologias;
CREATE TABLE users (
	id_user INT AUTO_INCREMENT PRIMARY KEY,
    name_user VARCHAR(255)NOT NULL UNIQUE,
    lastName_user VARCHAR(255)NOT NULL UNIQUE,
    email_user VARCHAR(255) NOT NULL UNIQUE,
    password_user VARCHAR(255) NOT NULL
    );

CREATE TABLE product (
	id_product BIGINT PRIMARY KEY,
    name_product VARCHAR(255) NOT NULL,
    categoria_product VARCHAR(255) NOT NULL,
    precioCompra_product INT NOT NULL,
    precioVenta_product INT NOT NULL
    );
    

DELIMITER //
CREATE PROCEDURE InsertUser (
    IN p_name VARCHAR(255),
    IN p_lastName VARCHAR(255),
    IN p_email VARCHAR(255),
    IN p_password VARCHAR(255)
    )
BEGIN
	INSERT INTO USERS (name_user, lastName_user, email_user, password_user) VALUES (p_name, p_lastName, p_email, p_password);
END//
DELIMITER ;

DELIMITER //
CREATE PROCEDURE InsertProduct (
	IN p_id BIGINT,
    IN p_name VARCHAR(255),
    IN p_categoria VARCHAR(255),
    IN p_precioCompra INT,
    IN p_precioVenta INT
	)
BEGIN
	INSERT INTO product (id_product, name_product, categoria_product, precioCompra_product, precioVenta_product) VALUES (p_id, p_name, p_categoria, p_precioCompra, p_precioVenta0);
END//
DELIMITER ;

SELECT * FROM users;

