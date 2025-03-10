CREATE DATABASE IF NOT EXISTS project_mams_db;
USE project_mams_db;

DROP TABLE IF EXISTS clients;
CREATE TABLE IF NOT EXISTS clients (
    cli_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    cli_name VARCHAR(50) NOT NULL,
    cli_phone VARCHAR(25),
    cli_email VARCHAR(255),
    cli_city VARCHAR(50),
    cli_address VARCHAR(100),
    cli_archive DATETIME
);

DROP TABLE IF EXISTS receipts;
CREATE TABLE IF NOT EXISTS receipts (
    rec_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    rec_total_price DECIMAL(7,2) NOT NULL ,
    rec_date_sold DATETIME NOT NULL ,
    cli_id INT REFERENCES clients(cli_id) NOT NULL
);

DROP TABLE IF EXISTS products_types;
CREATE TABLE IF NOT EXISTS products_types (
    pty_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    pty_name VARCHAR(50) NOT NULL,
    pty_archive DATETIME
);

DROP TABLE IF EXISTS products_categories;
CREATE TABLE IF NOT EXISTS products_categories (
    pca_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    pca_name VARCHAR(50) NOT NULL,
    pca_archive DATETIME
);

DROP TABLE IF EXISTS products_shapes;
CREATE TABLE IF NOT EXISTS products_shapes (
    psh_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    psh_name VARCHAR(50) NOT NULL,
    psh_archive DATETIME
);

DROP TABLE IF EXISTS products_details;
CREATE TABLE IF NOT EXISTS products_details (
    pde_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    pde_name VARCHAR(25),
    pde_weight INT,
    pde_archive DATETIME,
    psh_id INT REFERENCES products_shapes(psh_id),
    pty_id INT REFERENCES products_types(pty_id),
    pca_id INT REFERENCES products_categories(pca_id)
);

DROP TABLE IF EXISTS products_names;
CREATE TABLE IF NOT EXISTS products_names (
    pna_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    pna_name VARCHAR(50) NOT NULL,
    pna_archive DATETIME,
    pde_id INT REFERENCES products_details(pde_id)
);

DROP TABLE IF EXISTS beehives;
CREATE TABLE IF NOT EXISTS beehives (
    bhi_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    bhi_name VARCHAR(50) NOT NULL,
    bhi_archive DATETIME
);

DROP TABLE IF EXISTS products_lots;
CREATE TABLE IF NOT EXISTS products_lots (
    plo_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    plo_nbr INT NOT NULL,
    plo_year INT NOT NULL,
    plo_archive DATETIME,
    bhi_id INT REFERENCES beehives(bhi_id)
);

DROP TABLE IF EXISTS products;
CREATE TABLE IF NOT EXISTS products (
    pro_id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    pro_quantity INT NOT NULL,
    pro_unity_price DECIMAL(7,2),
    pro_archive DATETIME,
    pna_id INT REFERENCES products_names(pna_id),
    plo_id INT REFERENCES products_lots(plo_id)
);