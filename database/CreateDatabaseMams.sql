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
CREATE INDEX idx_entity_name ON entities(entity_name);

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
CREATE INDEX idx_receipt_number ON receipts(receipt_number);
CREATE INDEX idx_receipt_total_price ON receipts(receipt_total_price);
CREATE INDEX idx_receipt_date_created ON receipts(receipt_date_created);

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
CREATE INDEX idx_product_type_name ON products_types(product_type_name);

CREATE TABLE IF NOT EXISTS products_categories (
    product_category_id INT PRIMARY KEY AUTO_INCREMENT,
    product_category_name VARCHAR(100) NOT NULL,
    product_category_archive DATE
);
CREATE INDEX idx_product_category_name ON products_categories(product_category_name);

CREATE TABLE IF NOT EXISTS products_shapes (
    product_shape_id INT PRIMARY KEY AUTO_INCREMENT,
    product_shape_name VARCHAR(100) NOT NULL,
    product_shape_archive DATE
);
CREATE INDEX idx_product_shape_name ON products_shapes(product_shape_name);

CREATE TABLE IF NOT EXISTS beehives (
    beehive_id INT PRIMARY KEY AUTO_INCREMENT,
    beehive_name VARCHAR(50) NOT NULL,
    beehive_archive DATE
);
CREATE INDEX idx_beehive_name ON beehives(beehive_name);

CREATE TABLE IF NOT EXISTS products_lots (
    product_lot_id INT PRIMARY KEY AUTO_INCREMENT,
    product_lot_name VARCHAR(50) NOT NULL,
    product_lot_year INT NOT NULL,
    product_lot_archive DATE,
    fk_beehive_id INT,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id)
);
CREATE INDEX idx_product_lot_name ON products_lots(product_lot_name);
CREATE INDEX idx_product_lot_year ON products_lots(product_lot_year);

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
CREATE INDEX idx_product_name ON products(product_name);
CREATE INDEX idx_product_weight ON products(product_weight);

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
CREATE INDEX idx_receipt_product_quantity ON receipts_products(receipt_product_quantity);
CREATE INDEX idx_receipt_product_unity_price ON receipts_products(receipt_product_unity_price);

-- Insert default data to avoid NULL values in foreign keys
INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES
('', '1901-01-01');
INSERT INTO beehives (beehive_name, beehive_archive) VALUES
('', '1901-01-01');
INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id, product_lot_archive) VALUES
('', 0, 1, '1901-01-01');

-- Insert the user data for invoice generation
INSERT INTO entities (entity_id, entity_name, entity_phone, entity_email, entity_city, entity_address, entity_archive) VALUES
(1, 'Corinne Thumelin', '026 667 11 78', '', '1773 Russy', 'Rte de l''Ecole 12', '1901-01-01');