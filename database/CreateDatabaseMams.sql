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
    entity_archive DATE
);

CREATE TABLE IF NOT EXISTS suppliers (
    supplier_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL,
    FOREIGN KEY (fk_entity_id) REFERENCES entities(entity_id)
);

CREATE TABLE IF NOT EXISTS clients (
    client_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL,
    FOREIGN KEY (fk_entity_id) REFERENCES entities(entity_id)
);

CREATE TABLE IF NOT EXISTS receipts (
    receipt_id INT PRIMARY KEY AUTO_INCREMENT,
    receipt_number VARCHAR(50) NOT NULL,
    receipt_total_price DECIMAL(9,2) NOT NULL,
    receipt_date_created DATE NOT NULL
);

CREATE TABLE IF NOT EXISTS receipts_suppliers(
    fk_receipt_id INT NOT NULL,
    fk_supplier_id INT NOT NULL,
    FOREIGN KEY (fk_receipt_id) REFERENCES receipts(receipt_id),
    FOREIGN KEY (fk_supplier_id) REFERENCES suppliers(supplier_id)
);

CREATE TABLE IF NOT EXISTS receipts_clients(
    fk_receipt_id INT NOT NULL,
    fk_client_id INT NOT NULL,
    FOREIGN KEY (fk_receipt_id) REFERENCES receipts(receipt_id),
    FOREIGN KEY (fk_client_id) REFERENCES clients(client_id)
);

CREATE TABLE IF NOT EXISTS products_types (
    product_type_id INT PRIMARY KEY AUTO_INCREMENT,
    product_type_name VARCHAR(100) NOT NULL,
    product_type_archive DATE
);

CREATE TABLE IF NOT EXISTS products_categories (
    product_category_id INT PRIMARY KEY AUTO_INCREMENT,
    product_category_name VARCHAR(100) NOT NULL,
    product_category_archive DATE
);

CREATE TABLE IF NOT EXISTS products_shapes (
    product_shape_id INT PRIMARY KEY AUTO_INCREMENT,
    product_shape_name VARCHAR(100) NOT NULL,
    product_shape_archive DATE
);

CREATE TABLE IF NOT EXISTS beehives (
    beehive_id INT PRIMARY KEY AUTO_INCREMENT,
    beehive_name VARCHAR(50) NOT NULL,
    beehive_archive DATE
);

CREATE TABLE IF NOT EXISTS products_lots (
    product_lot_id INT PRIMARY KEY AUTO_INCREMENT,
    product_lot_name VARCHAR(50) NOT NULL,
    product_lot_year INT NOT NULL,
    product_lot_archive DATE,
    fk_beehive_id INT,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id)
);

CREATE TABLE IF NOT EXISTS products (
    product_id INT PRIMARY KEY AUTO_INCREMENT,
    product_name VARCHAR(100) NOT NULL,
    product_weight INT,
    product_archive DATE,
    fk_product_type_id INT NOT NULL,
    fk_product_category_id INT NOT NULL,
    fk_product_shape_id INT,
    FOREIGN KEY (fk_product_type_id) REFERENCES products_types(product_type_id),
    FOREIGN KEY (fk_product_category_id) REFERENCES products_categories(product_category_id),
    FOREIGN KEY (fk_product_shape_id) REFERENCES products_shapes(product_shape_id)
);

CREATE TABLE IF NOT EXISTS receipts_products (
    receipt_product_id INT PRIMARY KEY AUTO_INCREMENT,
    receipt_product_quantity INT NOT NULL,
    receipt_product_unity_price DECIMAL(9,2) NOT NULL,
    fk_product_id INT NOT NULL,
    fk_receipt_id INT NOT NULL,
    fk_product_lot_id INT,
    FOREIGN KEY (fk_product_id) REFERENCES products(product_id),
    FOREIGN KEY (fk_receipt_id) REFERENCES receipts(receipt_id),
    FOREIGN KEY (fk_product_lot_id) REFERENCES products_lots(product_lot_id)
);

-- Insert default data to avoid NULL values in foreign keys
INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES
('', '1901-01-01');
INSERT INTO beehives (beehive_name, beehive_archive) VALUES
('', '1901-01-01');
INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id, product_lot_archive) VALUES
('', 0, 1, '1901-01-01');