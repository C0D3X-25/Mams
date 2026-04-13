-- DROP DATABASE IF EXISTS mams_db;
CREATE DATABASE IF NOT EXISTS mams_db
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;
USE mams_db;

-- Set default charset for this session
SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS entities (
    entity_id INT PRIMARY KEY AUTO_INCREMENT,
    entity_name VARCHAR(50) NOT NULL,
    entity_phone VARCHAR(25),
    entity_email VARCHAR(255),
    entity_city VARCHAR(50),
    entity_address VARCHAR(255),
    entity_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_entity_name ON entities(entity_name);

CREATE TABLE IF NOT EXISTS suppliers (
    supplier_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL,
    FOREIGN KEY (fk_entity_id) REFERENCES entities(entity_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS clients (
    client_id INT PRIMARY KEY AUTO_INCREMENT,
    fk_entity_id INT NOT NULL,
    FOREIGN KEY (fk_entity_id) REFERENCES entities(entity_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS receipts (
    receipt_id INT PRIMARY KEY AUTO_INCREMENT,
    receipt_number VARCHAR(50) NOT NULL,
    receipt_total_price DECIMAL(9,2) NOT NULL,
    receipt_date_created DATE NOT NULL
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_receipt_number ON receipts(receipt_number);
CREATE INDEX idx_receipt_total_price ON receipts(receipt_total_price);
CREATE INDEX idx_receipt_date_created ON receipts(receipt_date_created);

CREATE TABLE IF NOT EXISTS receipts_suppliers(
    fk_receipt_id INT NOT NULL,
    fk_supplier_id INT NOT NULL,
    FOREIGN KEY (fk_receipt_id) REFERENCES receipts(receipt_id),
    FOREIGN KEY (fk_supplier_id) REFERENCES suppliers(supplier_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS receipts_clients(
    fk_receipt_id INT NOT NULL,
    fk_client_id INT NOT NULL,
    FOREIGN KEY (fk_receipt_id) REFERENCES receipts(receipt_id),
    FOREIGN KEY (fk_client_id) REFERENCES clients(client_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS products_types (
    product_type_id INT PRIMARY KEY AUTO_INCREMENT,
    product_type_name VARCHAR(100) NOT NULL,
    product_type_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_product_type_name ON products_types(product_type_name);

CREATE TABLE IF NOT EXISTS products_categories (
    product_category_id INT PRIMARY KEY AUTO_INCREMENT,
    product_category_name VARCHAR(100) NOT NULL,
    product_category_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_product_category_name ON products_categories(product_category_name);

CREATE TABLE IF NOT EXISTS products_shapes (
    product_shape_id INT PRIMARY KEY AUTO_INCREMENT,
    product_shape_name VARCHAR(100) NOT NULL,
    product_shape_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_product_shape_name ON products_shapes(product_shape_name);

CREATE TABLE IF NOT EXISTS regions (
    region_id INT PRIMARY KEY AUTO_INCREMENT,
    region_name VARCHAR(100) NOT NULL,
    region_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_region_name ON regions(region_name);

CREATE TABLE IF NOT EXISTS beehives (
    beehive_id INT PRIMARY KEY AUTO_INCREMENT,
    beehive_name VARCHAR(50) NOT NULL,
    beehive_number VARCHAR(50),
    beehive_archive DATE,
    fk_region_id INT,
    FOREIGN KEY (fk_region_id) REFERENCES regions(region_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_beehive_name ON beehives(beehive_name);

CREATE TABLE IF NOT EXISTS products_lots (
    product_lot_id INT PRIMARY KEY AUTO_INCREMENT,
    product_lot_name VARCHAR(50) NOT NULL,
    product_lot_year INT NOT NULL,
    product_lot_archive DATE,
    fk_beehive_id INT,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
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
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
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
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_receipt_product_quantity ON receipts_products(receipt_product_quantity);
CREATE INDEX idx_receipt_product_unity_price ON receipts_products(receipt_product_unity_price);

CREATE TABLE IF NOT EXISTS dose_units (
    dose_unit_id INT PRIMARY KEY AUTO_INCREMENT,
    dose_unit_name VARCHAR(100) NOT NULL,
    dose_unit_archive DATE
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_dose_unit_name ON dose_units(dose_unit_name);

CREATE TABLE IF NOT EXISTS treatments (
    treatment_id INT PRIMARY KEY AUTO_INCREMENT,
    treatment_date DATE NOT NULL,
    treatment_hive_count INT NOT NULL,
    treatment_dose_per_hive DECIMAL(9,2) NOT NULL,
    fk_beehive_id INT NOT NULL,
    fk_product_id INT NOT NULL,
    fk_dose_unit_id INT,
    FOREIGN KEY (fk_beehive_id) REFERENCES beehives(beehive_id),
    FOREIGN KEY (fk_product_id) REFERENCES products(product_id),
    FOREIGN KEY (fk_dose_unit_id) REFERENCES dose_units(dose_unit_id)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE INDEX idx_treatment_date ON treatments(treatment_date);

-- User table for invoice generation (stores the app owner's information)
CREATE TABLE IF NOT EXISTS users (
    user_id INT PRIMARY KEY AUTO_INCREMENT,
    user_name VARCHAR(50) NOT NULL,
    user_phone VARCHAR(25),
    user_email VARCHAR(255),
    user_city VARCHAR(50),
    user_address VARCHAR(255)
) CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;

-- Insert default data to avoid NULL values in foreign keys
INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES
('', '1901-01-01');
INSERT INTO regions (region_name, region_archive) VALUES
('', '1901-01-01');

-- Insert all Swiss cantons (regions)
INSERT INTO regions (region_name) VALUES
('Argovie'),
('Appenzell Rhodes-Intérieures'),
('Appenzell Rhodes-Extérieures'),
('Berne'),
('Bâle-Campagne'),
('Bâle-Ville'),
('Fribourg'),
('Genève'),
('Glaris'),
('Grisons'),
('Jura'),
('Lucerne'),
('Neuchâtel'),
('Nidwald'),
('Obwald'),
('Saint-Gall'),
('Schaffhouse'),
('Soleure'),
('Schwyz'),
('Thurgovie'),
('Tessin'),
('Uri'),
('Vaud'),
('Valais'),
('Zoug'),
('Zurich');

INSERT INTO beehives (beehive_name, beehive_archive, fk_region_id) VALUES
('', '1901-01-01', 1);
INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id, product_lot_archive) VALUES
('', 0, 1, '1901-01-01');
