CREATE DATABASE IF NOT EXISTS test;
USE test;

DROP TABLE IF EXISTS inventory;
CREATE TABLE IF NOT EXISTS inventory(
    id INT PRIMARY KEY AUTO_INCREMENT NOT NULL,
    name VARCHAR(25) NOT NULL,
    quantity INT NOT NULL
);

INSERT INTO inventory (id, name, quantity)
VALUES (1, 'apple', 253),
       (2, 'orange', 150),
       (3, 'blueberries', 25),
       (4, 'banana', 62);