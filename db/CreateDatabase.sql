CREATE DATABASE IF NOT EXISTS mams_db;
USE mams_db;

DROP TABLE IF EXISTS clients;
CREATE TABLE IF NOT EXISTS clients (
    cli_id INT PRIMARY KEY AUTO_INCREMENT,
    cli_name VARCHAR(50) NOT NULL,
    cli_phone VARCHAR(25),
    cli_email VARCHAR(255),
    cli_city VARCHAR(50),
    cli_address VARCHAR(255),
    cli_archive DATETIME
);

DROP TABLE IF EXISTS receipts;
CREATE TABLE IF NOT EXISTS receipts (
    rec_id INT PRIMARY KEY AUTO_INCREMENT,
    rec_total_price DECIMAL(9,2) NOT NULL,
    rec_date_sold DATETIME NOT NULL,
    cli_id INT NOT NULL REFERENCES clients(cli_id)
);

DROP TABLE IF EXISTS products_types;
CREATE TABLE IF NOT EXISTS products_types (
    pty_id INT PRIMARY KEY AUTO_INCREMENT,
    pty_name VARCHAR(100) NOT NULL,
    pty_archive DATETIME
);

DROP TABLE IF EXISTS products_categories;
CREATE TABLE IF NOT EXISTS products_categories (
    pca_id INT PRIMARY KEY AUTO_INCREMENT,
    pca_name VARCHAR(100) NOT NULL,
    pca_archive DATETIME
);

DROP TABLE IF EXISTS products_shapes;
CREATE TABLE IF NOT EXISTS products_shapes (
    psh_id INT PRIMARY KEY AUTO_INCREMENT,
    psh_name VARCHAR(100) NOT NULL,
    psh_archive DATETIME
);

DROP TABLE IF EXISTS beehives;
CREATE TABLE IF NOT EXISTS beehives (
    bhi_id INT PRIMARY KEY AUTO_INCREMENT,
    bhi_name VARCHAR(50) NOT NULL,
    bhi_archive DATETIME
);

DROP TABLE IF EXISTS products_lots;
CREATE TABLE IF NOT EXISTS products_lots (
    plo_id INT PRIMARY KEY AUTO_INCREMENT,
    plo_nbr INT NOT NULL,
    plo_year INT NOT NULL,
    plo_archive DATETIME,
    bhi_id INT REFERENCES beehives(bhi_id)
);

DROP TABLE IF EXISTS products;
CREATE TABLE IF NOT EXISTS products (
    pro_id INT PRIMARY KEY AUTO_INCREMENT,
    pro_name VARCHAR(100) NOT NULL,
    pro_weight INT,
    pro_archive DATETIME,
    pca_id INT NOT NULL REFERENCES products_categories(pca_id),
    pty_id INT NOT NULL REFERENCES products_types(pty_id),
    psh_id INT REFERENCES products_shapes(psh_id),
    plo_id INT REFERENCES products_lots(plo_id)
);

DROP TABLE IF EXISTS receipts_products;
CREATE TABLE IF NOT EXISTS receipts_products (
    rpr_id INT PRIMARY KEY AUTO_INCREMENT,
    rpr_quantity INT,
    rpr_unity_price DECIMAL(9,2),
    pro_id INT NOT NULL REFERENCES products(pro_id),
    rec_id INT NOT NULL REFERENCES receipts(rec_id)
);