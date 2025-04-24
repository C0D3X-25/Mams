-- DROP DATABASE IF EXISTS mams_db;
CREATE DATABASE IF NOT EXISTS mams_db;
USE mams_db;

CREATE TABLE IF NOT EXISTS entities (
    entity_id INT PRIMARY KEY AUTO_INCREMENT,
    entity_name VARCHAR(50) NOT NULL,
    entity_phone VARCHAR(25),
    entity_email VARCHAR(255),
    entity_city VARCHAR(50),
    entity_address VARCHAR(255),
    entity_archive DATETIME
);

CREATE TABLE IF NOT EXISTS suppliers (
    supplier_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL REFERENCES entities(entity_id)
);

CREATE TABLE IF NOT EXISTS clients (
    client_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL REFERENCES entities(entity_id)
);

CREATE TABLE IF NOT EXISTS receipts (
    receipt_id INT PRIMARY KEY AUTO_INCREMENT,
    receipt_total_price DECIMAL(9,2) NOT NULL,
    receipt_date_sold DATETIME NOT NULL
);

CREATE TABLE IF NOT EXISTS suppliers_receipts(
    fk_supplier_id INT NOT NULL REFERENCES suppliers(supplier_id),
    fk_receipt_id INT NOT NULL REFERENCES receipts(receipt_id)
);

CREATE TABLE IF NOT EXISTS clients_receipts(
    fk_client_id INT NOT NULL REFERENCES clients(client_id),
    fk_receipt_id INT NOT NULL REFERENCES receipts(receipt_id)
);

CREATE TABLE IF NOT EXISTS products_types (
    product_type_id INT PRIMARY KEY AUTO_INCREMENT,
    product_type_name VARCHAR(100) NOT NULL,
    product_type_archive DATETIME
);

CREATE TABLE IF NOT EXISTS products_categories (
    product_category_id INT PRIMARY KEY AUTO_INCREMENT,
    product_category_name VARCHAR(100) NOT NULL,
    product_category_archive DATETIME
);

CREATE TABLE IF NOT EXISTS products_shapes (
    product_shape_id INT PRIMARY KEY AUTO_INCREMENT,
    product_shape_name VARCHAR(100) NOT NULL,
    product_shape_archive DATETIME
);

CREATE TABLE IF NOT EXISTS beehives (
    beehive_id INT PRIMARY KEY AUTO_INCREMENT,
    beehive_name VARCHAR(50) NOT NULL,
    beehive_archive DATETIME
);

CREATE TABLE IF NOT EXISTS products_lots (
    product_lot_id INT PRIMARY KEY AUTO_INCREMENT,
    product_lot_name VARCHAR(50) NOT NULL,
    product_lot_year INT NOT NULL,
    product_lot_archive DATETIME,
    fk_beehive_id INT REFERENCES beehives(beehive_id)
);

CREATE TABLE IF NOT EXISTS products (
    product_id INT PRIMARY KEY AUTO_INCREMENT,
    product_name VARCHAR(100) NOT NULL,
    product_weight INT,
    product_archive DATETIME,
    fk_product_type_id INT NOT NULL REFERENCES products_types(product_type_id),
    fk_product_category_id INT REFERENCES products_categories(product_category_id),
    fk_product_shape_id INT REFERENCES products_shapes(product_shape_id),
    fk_product_lot_id INT REFERENCES products_lots(product_lot_id)
);

CREATE TABLE IF NOT EXISTS receipts_products (
    receipt_product_id INT PRIMARY KEY AUTO_INCREMENT,
    receipt_product_quantity INT,
    receipt_product_unity_price DECIMAL(9,2),
    fk_product_id INT NOT NULL REFERENCES products(product_id),
    fk_receipt_id INT NOT NULL REFERENCES receipts(receipt_id)
);