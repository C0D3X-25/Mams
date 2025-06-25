DROP DATABASE IF EXISTS mams_db;
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


-- Insert data into entities (including mixed supplier-client entities)
INSERT INTO entities (entity_name, entity_phone, entity_email, entity_city, entity_address, entity_archive) VALUES
('Johnson Grocery', '+44 20 1234 5678', 'orders@johnsongrocery.com', 'London', '15 Market Street, EC1A 1BB', NULL),
('Natural Foods Ltd', '+44 20 2345 6789', 'purchasing@naturalfoods.co.uk', 'Birmingham', '27 Organic Way, B1 1TF', NULL),
('Green Health Store', '+44 20 3456 7890', 'contact@greenhealthstore.co.uk', 'Edinburgh', '8 Wellness Road, EH1 1TH', NULL),
('Wholesome Market', '+44 20 4567 8901', 'orders@wholesomemarket.com', 'Bristol', '53 Pure Street, BS1 4DF', NULL),
('Honey Harvest Suppliers', '+44 20 8765 4321', 'sales@honeyharvest.com', 'Manchester', '12 Supplier Street, M1 3AB', NULL),
('Bee World Distributors', '+44 20 9876 5432', 'info@beeworld.co.uk', 'Birmingham', '25 Distribution Road, B2 2CD', NULL),
('Sweet Nectar Imports', '+44 20 7654 3210', 'contact@sweetnectar.com', 'London', '8 Import Avenue, EC2 4EF', '2024-02-15'),
('Organic Bee Collective', '+44 20 6543 2109', 'hello@organicbee.co.uk', 'Edinburgh', '15 Green Lane, EH2 5GH', NULL),
-- Three entities that will be both suppliers and clients
('Dual Purpose Honey Co.', '+44 20 1111 2222', 'info@dualpurposehoney.com', 'Manchester', '10 Crossover Street, M2 3CD', NULL),
('Versatile Bee Products', '+44 20 3333 4444', 'sales@versatilebee.co.uk', 'Liverpool', '22 Flexible Road, L4 5EF', NULL),
('Hybrid Honey Traders', '+44 20 5555 6666', 'contact@hybridhoney.com', 'Glasgow', '7 Exchange Lane, G5 6GH', NULL);

-- Insert these same entities into both suppliers and clients
INSERT INTO suppliers (fk_entity_id) VALUES
(5), (6), (7), (8), (9), (10), (11);

INSERT INTO clients (fk_entity_id) VALUES
(1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11);

-- Insert data into products_types (same as previous)
INSERT INTO products_types (product_type_name, product_type_archive) VALUES
('Raw Honey', NULL),
('Creamed Honey', NULL),
('Infused Honey', NULL),
('Honey Comb', NULL),
('Propolis', NULL),
('Pollen', '2024-01-15'),
('Royal Jelly', NULL),
('Beeswax Products', NULL);

-- Insert data into products_categories (same as previous)
INSERT INTO products_categories (product_category_name, product_category_archive) VALUES
('Spring Collection', NULL),
('Summer Collection', NULL),
('Autumn Collection', NULL),
('Winter Collection', '2024-02-10'),
('Premium Collection', NULL),
('Organic Collection', NULL),
('Gift Collection', NULL),
('Natural Remedies', '2024-03-01');

-- Insert data into products_shapes (same as previous)
INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES
('Jar', NULL),
('Bottle', NULL),
('Hexagonal Jar', NULL),
('Square Jar', '2024-01-20'),
('Gift Box', NULL),
('Tube', NULL),
('Comb Frame', NULL),
('Honeycomb Section', NULL),
('Classic Jar', NULL),
('Mini Jar', NULL);

-- Insert data into beehives (same as previous)
INSERT INTO beehives (beehive_name, beehive_archive) VALUES
('Wildflower Meadow', NULL),
('Orchard Grove', NULL),
('Mountain Ridge', '2024-02-05'),
('Forest Edge', NULL),
('Lavender Fields', NULL),
('Riverside Hives', NULL),
('Valley Apiary', '2024-01-10'),
('Clover Fields', NULL);

-- Insert data into products_lots (same as previous)
INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id, product_lot_archive) VALUES
('101', 2024, 1, NULL),
('102', 2024, 2, NULL),
('103', 2024, 3, NULL),
('104', 2024, 4, NULL),
('105', 2024, 5, '2024-02-15'),
('106', 2024, 6, NULL),
('107', 2024, 7, NULL),
('108', 2024, 8, NULL),
('201', 2023, 1, '2024-03-05'),
('202', 2023, 2, NULL);

-- Insert data into products (same as previous)
INSERT INTO products (product_name, product_weight, fk_product_category_id, fk_product_type_id, fk_product_shape_id, product_archive) VALUES
('Wildflower Raw Honey', 500, 1, 1, 1, NULL),
('Acacia Creamed Honey', 350, 2, 2, 2, NULL),
('Lavender Infused Honey', 250, 5, 3, 5, '2024-02-20'),
('Pure Honeycomb'       , 400, 4, 4, 3, NULL),
('Premium Propolis Extract', 100, 8, 5, 4, NULL),
('Organic Bee Pollen'   , 200, 6, 6, 6, '2024-01-25'),
('Royal Jelly Premium'  , 50, 5, 7, 7, NULL),
('Natural Beeswax Candle Set', 150, 7, 8, 8, NULL),
('Heather Honey'        , 500, 3, 1, NULL, NULL),
('Orange Blossom Honey' , 250, 2, 1, NULL, NULL),
('Manuka Honey Special Reserve', 150, 5, 1, 1, NULL),
('Buckwheat Raw Honey'  , 500, 3, 1, 2, '2024-03-02'),
('Eucalyptus Honey'     , 350, 6, 1, 3, NULL),
('Linden Honey'         , 250, 1, 1, 4, NULL),
('Clover Honey'         , 500, 2, 1, 5, NULL),
('Beeswax Food Wraps'   , NULL, 7, 8, 6, NULL),
('Propolis Tincture'    , 30, 8, 5, 7, NULL),
('Honeycomb Gift Box'   , 300, 7, 4, 8, '2024-02-28'),
('Chestnut Honey'       , 500, 3, 1, 9, NULL),
('Thyme Honey'          , NULL, 1, 1, 10, NULL);

-- Insert data into receipts
INSERT INTO receipts (receipt_total_price, receipt_date_created) VALUES
(124.50, '2025-01-15'),
(76.80, '2025-01-22'),
(198.25, '2021-01-30'),
(45.00, '2024-02-05'),
(312.75, '2024-02-12'),
(87.20, '2022-02-20'),
(156.40, '2025-02-28'),
(234.60, '2024-03-05'),
(67.90, '2020-03-10'),
(189.30, '2021-03-12');

-- Insert data into receipts_suppliers
INSERT INTO receipts_suppliers (fk_supplier_id, fk_receipt_id) VALUES
(1, 1), (2, 2), (3, 3), (4, 4),
(5, 5);

-- Insert data into receipts_clients
INSERT INTO receipts_clients (fk_client_id, fk_receipt_id) VALUES
(6, 6), (7, 7), (8, 8),
(9, 9), (10, 10);

-- Insert data into receipts_products
INSERT INTO receipts_products (receipt_product_quantity, receipt_product_unity_price, fk_product_id, fk_receipt_id, fk_product_lot_id) VALUES
(3, 18.50, 1, 1, 1),
(2, 34.50, 11, 1, 2),
(1, 24.50, 5, 2, 3),
(2, 26.15, 6, 2, 4),
(5, 18.50, 1, 3, 5),
(2, 32.75, 13, 3, 6),
(3, 15.00, 20, 4, 7),
(10, 18.50, 1, 5, 8),
(5, 14.75, 9, 5, 9),
(2, 19.50, 12, 5, 10),
(1, 15.90, 15, 6, NULL),
(2, 19.75, 17, 6, 1),
(5, 16.80, 4, 7, 2),
(3, 23.40, 18, 7, 3),
(8, 18.50, 1, 8, 4),
(4, 21.90, 19, 8, 5),
(2, 0, 11, 9, 6),
(6, 17.50, 14, 10, 7),
(4, 22.45, 16, 10, 8);