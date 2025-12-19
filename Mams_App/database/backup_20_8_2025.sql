-- MySqlBackup.NET 2.3.9.0
-- Dump Time: 2025-08-20 10:57:15
-- --------------------------------------
-- Server version 8.0.42 MySQL Community Server - GPL
CREATE DATABASE IF NOT EXISTS mams_db;
USE mams_db;


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of beehives
-- 

DROP TABLE IF EXISTS `beehives`;
CREATE TABLE IF NOT EXISTS `beehives` (
  `beehive_id` int NOT NULL AUTO_INCREMENT,
  `beehive_name` varchar(50) NOT NULL,
  `beehive_archive` date DEFAULT NULL,
  PRIMARY KEY (`beehive_id`),
  KEY `idx_beehive_name` (`beehive_name`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table beehives
-- 

/*!40000 ALTER TABLE `beehives` DISABLE KEYS */;
INSERT INTO `beehives`(`beehive_id`,`beehive_name`,`beehive_archive`) VALUES(1,'','1901-01-01 00:00:00'),(2,'La Côte',NULL),(3,'Le Laret',NULL),(4,'Petit Belmont',NULL);
/*!40000 ALTER TABLE `beehives` ENABLE KEYS */;

-- 
-- Definition of entities
-- 

DROP TABLE IF EXISTS `entities`;
CREATE TABLE IF NOT EXISTS `entities` (
  `entity_id` int NOT NULL AUTO_INCREMENT,
  `entity_name` varchar(50) NOT NULL,
  `entity_phone` varchar(25) DEFAULT NULL,
  `entity_email` varchar(255) DEFAULT NULL,
  `entity_city` varchar(50) DEFAULT NULL,
  `entity_address` varchar(255) DEFAULT NULL,
  `entity_archive` date DEFAULT NULL,
  PRIMARY KEY (`entity_id`),
  KEY `idx_entity_name` (`entity_name`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table entities
-- 

/*!40000 ALTER TABLE `entities` DISABLE KEYS */;
INSERT INTO `entities`(`entity_id`,`entity_name`,`entity_phone`,`entity_email`,`entity_city`,`entity_address`,`entity_archive`) VALUES(1,'Bluette','','','','',NULL),(2,'Roggen','','','','',NULL),(3,'Ruchat','','','','',NULL),(4,'Espace Abeilles','','','','',NULL),(5,'Carmelina','','','','',NULL),(6,'Chandines','','','','',NULL),(7,'Arbothévoz','','','','',NULL),(8,'Particuliers','','','','',NULL),(9,'Rithner','','','','',NULL),(10,'Bienen Meier','','','','',NULL),(11,'Apimat','','','','',NULL),(12,'Landi','','','','',NULL),(13,'Marché Ressudens 10.05.25','','','','',NULL),(14,'Marco location','','','','',NULL),(15,'Nicolas location','','','','',NULL),(16,'Corso Ivrea','','','','',NULL),(17,'JUMBO','','','','',NULL),(18,'Bastella','','','','',NULL),(19,'MIGROS','','','','',NULL),(20,'TEMPONNEMOI.COM','','','','',NULL),(21,'Frais divers','','','','',NULL),(22,'smartphoto','','','','',NULL),(23,'VBS','','','','',NULL),(24,'L''PIXL','026 675 53 90','info@lpixl.ch','1580 Avenches','Rue Bibracte 4a',NULL),(25,'ROUTE D''OR F','','','','',NULL),(26,'MANOR','','','','',NULL),(27,'Rue des Arts sàrl','','','1870 Monthey','Rue de Coppet 2',NULL),(28,'Ocres de France','0033 4 90 74 63 82','www.ocres-de-france.com','F 84400 Apt','Ch des Ocriers 200',NULL),(29,'Droguerie du Portail','026 660 25 18','','1530 Payerne','Grand Rue 64',NULL),(30,'SAR','','','','',NULL),(31,'COOP','','','','',NULL),(32,'Schilliger','','','','',NULL),(33,'FVA','','','','',NULL),(34,'Kurt Nobs','079 252 69 52','','1583 Villarepos','La Solitude 5',NULL),(35, 'Corinne Thumelin', '+26 667 11 78', '', '1773 Russy', 'Rte de l''Ecole 12', '1901-01-01');
/*!40000 ALTER TABLE `entities` ENABLE KEYS */;

-- 
-- Definition of clients
-- 

DROP TABLE IF EXISTS `clients`;
CREATE TABLE IF NOT EXISTS `clients` (
  `client_id` int NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int NOT NULL,
  PRIMARY KEY (`client_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `clients_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table clients
-- 

/*!40000 ALTER TABLE `clients` DISABLE KEYS */;
INSERT INTO `clients`(`client_id`,`fk_entity_id`) VALUES(1,1),(5,2),(4,3),(3,4),(2,5),(7,6),(8,7),(11,8),(12,9),(6,13),(9,14),(10,15);
/*!40000 ALTER TABLE `clients` ENABLE KEYS */;

-- 
-- Definition of products_categories
-- 

DROP TABLE IF EXISTS `products_categories`;
CREATE TABLE IF NOT EXISTS `products_categories` (
  `product_category_id` int NOT NULL AUTO_INCREMENT,
  `product_category_name` varchar(100) NOT NULL,
  `product_category_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_category_id`),
  KEY `idx_product_category_name` (`product_category_name`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_categories
-- 

/*!40000 ALTER TABLE `products_categories` DISABLE KEYS */;
INSERT INTO `products_categories`(`product_category_id`,`product_category_name`,`product_category_archive`) VALUES(1,'Miel',NULL),(2,'Bougie/cire',NULL),(3,'Savon',NULL),(4,'Divers',NULL),(10,'Matériel entretien',NULL),(11,'Matériel colonies',NULL),(12,'Administratif',NULL),(13,'Cire','2025-08-19 00:00:00'),(14,'Frais divers/port',NULL),(15,'Bien-être Apicultrice',NULL),(16,'Formation/cours',NULL);
/*!40000 ALTER TABLE `products_categories` ENABLE KEYS */;

-- 
-- Definition of products_lots
-- 

DROP TABLE IF EXISTS `products_lots`;
CREATE TABLE IF NOT EXISTS `products_lots` (
  `product_lot_id` int NOT NULL AUTO_INCREMENT,
  `product_lot_name` varchar(50) NOT NULL,
  `product_lot_year` int NOT NULL,
  `product_lot_archive` date DEFAULT NULL,
  `fk_beehive_id` int DEFAULT NULL,
  PRIMARY KEY (`product_lot_id`),
  KEY `fk_beehive_id` (`fk_beehive_id`),
  KEY `idx_product_lot_name` (`product_lot_name`),
  KEY `idx_product_lot_year` (`product_lot_year`),
  CONSTRAINT `products_lots_ibfk_1` FOREIGN KEY (`fk_beehive_id`) REFERENCES `beehives` (`beehive_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_lots
-- 

/*!40000 ALTER TABLE `products_lots` DISABLE KEYS */;
INSERT INTO `products_lots`(`product_lot_id`,`product_lot_name`,`product_lot_year`,`product_lot_archive`,`fk_beehive_id`) VALUES(1,'',0,'1901-01-01 00:00:00',1),(2,'LL125',2025,NULL,3),(3,'LL225',2025,NULL,3),(4,'LC125',2025,NULL,2),(5,'LC225',2025,NULL,2),(6,'L124',2024,NULL,3),(7,'L224',2024,NULL,3),(8,'L324',2024,NULL,3),(9,'L241',2024,NULL,2),(10,'L242',2024,NULL,2),(11,'L243',2024,NULL,2),(12,'L123',2023,NULL,3),(13,'L223',2023,NULL,3),(14,'L231',2023,NULL,2),(15,'L232',2023,NULL,2),(16,'L1.25',2025,NULL,3),(17,'LCCR25',2025,NULL,2),(18,'LLCR25',2025,NULL,3);
/*!40000 ALTER TABLE `products_lots` ENABLE KEYS */;

-- 
-- Definition of products_shapes
-- 

DROP TABLE IF EXISTS `products_shapes`;
CREATE TABLE IF NOT EXISTS `products_shapes` (
  `product_shape_id` int NOT NULL AUTO_INCREMENT,
  `product_shape_name` varchar(100) NOT NULL,
  `product_shape_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_shape_id`),
  KEY `idx_product_shape_name` (`product_shape_name`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_shapes
-- 

/*!40000 ALTER TABLE `products_shapes` DISABLE KEYS */;
INSERT INTO `products_shapes`(`product_shape_id`,`product_shape_name`,`product_shape_archive`) VALUES(1,'','1901-01-01 00:00:00');
/*!40000 ALTER TABLE `products_shapes` ENABLE KEYS */;

-- 
-- Definition of products
-- 

DROP TABLE IF EXISTS `products`;
CREATE TABLE IF NOT EXISTS `products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `product_name` varchar(100) NOT NULL,
  `product_weight` int DEFAULT NULL,
  `product_archive` date DEFAULT NULL,
  `fk_product_type_id` int NOT NULL,
  `fk_product_category_id` int NOT NULL,
  `fk_product_shape_id` int DEFAULT NULL,
  PRIMARY KEY (`product_id`),
  KEY `fk_product_type_id` (`fk_product_type_id`),
  KEY `fk_product_category_id` (`fk_product_category_id`),
  KEY `fk_product_shape_id` (`fk_product_shape_id`),
  KEY `idx_product_name` (`product_name`),
  KEY `idx_product_weight` (`product_weight`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`fk_product_type_id`) REFERENCES `products_types` (`product_type_id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`fk_product_category_id`) REFERENCES `products_categories` (`product_category_id`),
  CONSTRAINT `products_ibfk_3` FOREIGN KEY (`fk_product_shape_id`) REFERENCES `products_shapes` (`product_shape_id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products
-- 

/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products`(`product_id`,`product_name`,`product_weight`,`product_archive`,`fk_product_type_id`,`fk_product_category_id`,`fk_product_shape_id`) VALUES(1,'Miel 125g',125,NULL,1,1,1),(2,'Miel 250g',250,NULL,1,1,1),(3,'Miel 500g',500,NULL,1,1,1),(4,'Miel 1000g',1000,NULL,1,1,1),(5,'Bocaux',0,NULL,1,1,1),(6,'Etiquette',0,NULL,1,1,1),(7,'Moules bougies',0,NULL,1,2,1),(8,'Mèches bougies',0,NULL,1,2,1),(9,'Matière première',0,NULL,1,3,1),(10,'Tampon',0,NULL,1,3,1),(11,'Sacs',0,NULL,1,4,1),(12,'Carte/ visite',0,NULL,1,4,1),(13,'Emballage présentation',0,NULL,1,1,1),(14,'Peinture',0,NULL,2,10,1),(15,'Réparation',0,NULL,2,10,1),(16,'Nourissement',0,NULL,2,11,1),(17,'Traitement',0,NULL,2,11,1),(18,'Matériel apicole',0,NULL,2,11,1),(19,'Administratif',0,NULL,2,12,1),(20,'Bougie alvéoles grandes',0,NULL,1,2,1),(21,'Bougie alvéoles petites',0,NULL,1,2,1),(22,'Bougie 1001 Fleurs',0,NULL,1,2,1),(23,'Bougie longue',0,NULL,1,2,1),(24,'Bougie torsadée',0,NULL,1,2,1),(25,'Bougie ruche',0,NULL,1,2,1),(26,'Bougie hibou',0,NULL,1,2,1),(27,'Bougie florette',0,NULL,1,2,1),(28,'Bougie cire/béton',0,NULL,1,2,1),(29,'Cire bricolage',0,NULL,1,2,1),(30,'Bougie cire gaufrée',0,NULL,1,2,1),(31,'API''SAVON laurier',70,NULL,1,3,1),(32,'Miel trio 125',125,NULL,1,1,1),(33,'Miel duo 125',125,NULL,1,1,1),(34,'Miel duo 250',250,NULL,1,1,1),(35,'Miel en rayons',0,NULL,1,1,1),(36,'Décorations cire',0,NULL,1,2,1),(37,'Cire brut',0,NULL,1,2,1),(38,'Taxes/douane/livraison',0,NULL,1,14,1),(39,'Matériel décorations',0,NULL,1,4,1),(40,'Logo CT',0,NULL,1,4,1),(41,'Livres',0,NULL,2,16,1),(42,'Moules décos cire',0,NULL,1,2,1),(43,'Soins santé',0,NULL,2,15,1),(44,'Béton créatif',0,NULL,1,2,1),(45,'Contrôle miel',0,NULL,1,1,1),(46,'Reine Séléction',0,NULL,2,11,1);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;

-- 
-- Definition of products_types
-- 

DROP TABLE IF EXISTS `products_types`;
CREATE TABLE IF NOT EXISTS `products_types` (
  `product_type_id` int NOT NULL AUTO_INCREMENT,
  `product_type_name` varchar(100) NOT NULL,
  `product_type_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_type_id`),
  KEY `idx_product_type_name` (`product_type_name`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_types
-- 

/*!40000 ALTER TABLE `products_types` DISABLE KEYS */;
INSERT INTO `products_types`(`product_type_id`,`product_type_name`,`product_type_archive`) VALUES(1,'Production',NULL),(2,'Exploitation',NULL);
/*!40000 ALTER TABLE `products_types` ENABLE KEYS */;

-- 
-- Definition of receipts
-- 

DROP TABLE IF EXISTS `receipts`;
CREATE TABLE IF NOT EXISTS `receipts` (
  `receipt_id` int NOT NULL AUTO_INCREMENT,
  `receipt_number` varchar(50) NOT NULL,
  `receipt_total_price` decimal(9,2) NOT NULL,
  `receipt_date_created` date NOT NULL,
  PRIMARY KEY (`receipt_id`),
  KEY `idx_receipt_number` (`receipt_number`),
  KEY `idx_receipt_total_price` (`receipt_total_price`),
  KEY `idx_receipt_date_created` (`receipt_date_created`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts
-- 

/*!40000 ALTER TABLE `receipts` DISABLE KEYS */;
INSERT INTO `receipts`(`receipt_id`, `receipt_total_price`,`receipt_date_created`) VALUES(1,101.00,'2025-06-11 00:00:00'),(2,142.50,'2025-05-29 00:00:00'),(3,6.80,'2025-05-12 00:00:00'),(4,87.00,'2025-05-28 00:00:00'),(5,322.50,'2025-05-28 00:00:00'),(9,698.16,'2025-05-10 00:00:00'),(10,87.00,'2025-05-28 00:00:00'),(11,285.00,'2025-07-29 00:00:00'),(12,116.00,'2025-07-31 00:00:00'),(13,0.00,'2025-06-04 00:00:00'),(14,0.00,'2025-08-11 00:00:00'),(15,0.00,'2025-07-30 00:00:00'),(16,0.00,'2025-05-29 00:00:00'),(17,0.00,'2025-06-11 00:00:00'),(18,0.00,'2025-07-31 00:00:00'),(19,143.70,'2025-08-08 00:00:00'),(20,11.90,'2025-06-16 00:00:00'),(21,78.00,'2025-05-30 00:00:00'),(22,330.00,'2025-05-31 00:00:00'),(23,0.00,'2025-06-01 00:00:00'),(24,22.00,'2025-06-20 00:00:00'),(25,85.00,'2025-07-01 00:00:00'),(26,44.00,'2025-07-21 00:00:00'),(27,22.00,'2025-08-12 00:00:00'),(28,50.00,'2025-05-23 00:00:00'),(29,0.00,'2025-05-24 00:00:00'),(30,55.00,'2025-06-04 00:00:00'),(31,24.00,'2025-06-27 00:00:00'),(32,13.00,'2025-07-21 00:00:00'),(33,0.00,'2025-07-27 00:00:00'),(34,98.00,'2025-08-06 00:00:00'),(35,44.00,'2025-08-12 00:00:00'),(36,20.00,'2025-03-27 00:00:00'),(37,51.45,'2025-01-20 00:00:00'),(38,65.60,'2025-01-24 00:00:00'),(39,19.45,'2025-01-30 00:00:00'),(40,21.80,'2025-02-01 00:00:00'),(41,31.95,'2025-02-04 00:00:00'),(42,19.95,'2025-02-13 00:00:00'),(43,4.60,'2025-02-13 00:00:00'),(44,9.90,'2025-02-13 00:00:00'),(45,23.60,'2025-02-14 00:00:00'),(46,78.80,'2025-02-27 00:00:00'),(47,302.70,'2025-02-28 00:00:00'),(48,14.95,'2025-03-06 00:00:00'),(49,124.50,'2025-03-11 00:00:00'),(50,23.45,'2025-03-17 00:00:00'),(51,61.85,'2025-03-27 00:00:00'),(52,212.60,'2025-03-27 00:00:00'),(53,16.00,'2025-03-27 00:00:00'),(54,11.90,'2025-03-27 00:00:00'),(55,7.45,'2025-03-27 00:00:00'),(56,23.45,'2025-03-29 00:00:00'),(57,102.20,'2025-03-29 00:00:00'),(58,49.60,'2025-04-10 00:00:00'),(59,14.95,'2025-04-30 00:00:00'),(60,30.30,'2025-05-02 00:00:00'),(61,2.95,'2025-05-01 00:00:00'),(62,30.00,'2025-05-10 00:00:00'),(63,168.40,'2025-05-24 00:00:00'),(64,13.50,'2025-05-26 00:00:00'),(65,32.40,'2025-06-02 00:00:00'),(66,179.60,'2025-06-09 00:00:00'),(67,42.00,'2025-06-18 00:00:00'),(68,75.00,'2025-07-11 00:00:00'),(69,12.00,'2025-07-28 00:00:00');
/*!40000 ALTER TABLE `receipts` ENABLE KEYS */;

-- 
-- Definition of receipts_clients
-- 

DROP TABLE IF EXISTS `receipts_clients`;
CREATE TABLE IF NOT EXISTS `receipts_clients` (
  `fk_receipt_id` int NOT NULL,
  `fk_client_id` int NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_client_id` (`fk_client_id`),
  CONSTRAINT `receipts_clients_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_clients_ibfk_2` FOREIGN KEY (`fk_client_id`) REFERENCES `clients` (`client_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_clients
-- 

/*!40000 ALTER TABLE `receipts_clients` DISABLE KEYS */;
INSERT INTO `receipts_clients`(`fk_receipt_id`,`fk_client_id`) VALUES(2,2),(4,4),(5,5),(1,1),(10,1),(11,7),(12,1),(13,8),(14,8),(15,9),(16,9),(17,10),(18,10),(21,11),(25,11),(27,11),(28,11),(29,11),(22,11),(23,11),(30,11),(24,11),(31,11),(26,11),(32,11),(33,11),(34,11),(35,11),(36,12),(3,3),(19,3),(20,3),(9,6);
/*!40000 ALTER TABLE `receipts_clients` ENABLE KEYS */;

-- 
-- Definition of receipts_products
-- 

DROP TABLE IF EXISTS `receipts_products`;
CREATE TABLE IF NOT EXISTS `receipts_products` (
  `receipt_product_id` int NOT NULL AUTO_INCREMENT,
  `receipt_product_quantity` int NOT NULL,
  `receipt_product_unity_price` decimal(9,2) NOT NULL,
  `fk_product_id` int NOT NULL,
  `fk_receipt_id` int NOT NULL,
  `fk_product_lot_id` int DEFAULT NULL,
  PRIMARY KEY (`receipt_product_id`),
  KEY `fk_product_id` (`fk_product_id`),
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_product_lot_id` (`fk_product_lot_id`),
  KEY `idx_receipt_product_quantity` (`receipt_product_quantity`),
  KEY `idx_receipt_product_unity_price` (`receipt_product_unity_price`),
  CONSTRAINT `receipts_products_ibfk_1` FOREIGN KEY (`fk_product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `receipts_products_ibfk_2` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_products_ibfk_3` FOREIGN KEY (`fk_product_lot_id`) REFERENCES `products_lots` (`product_lot_id`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_products
-- 

/*!40000 ALTER TABLE `receipts_products` DISABLE KEYS */;
INSERT INTO `receipts_products`(`receipt_product_id`,`receipt_product_quantity`,`receipt_product_unity_price`,`fk_product_id`,`fk_receipt_id`,`fk_product_lot_id`) VALUES(3,15,9.50,3,2,4),(8,6,9.50,3,4,2),(9,6,5.00,2,4,2),(12,10,18.00,4,5,2),(13,15,9.50,3,5,2),(22,8,9.50,3,1,2),(23,5,5.00,2,1,2),(24,6,9.50,3,10,2),(25,6,5.00,2,10,2),(26,5,9.50,3,11,4),(27,25,9.50,3,11,5),(28,8,9.50,3,12,3),(29,3,5.00,2,12,2),(30,5,5.00,2,12,3),(31,10,0.00,3,13,4),(32,5,0.00,3,14,5),(227,3,0.00,3,15,5),(228,2,0.00,3,16,4),(229,3,0.00,3,17,2),(230,4,0.00,3,18,3),(244,6,13.00,3,21,2),(248,5,11.00,3,25,2),(249,5,6.00,2,25,2),(251,1,11.00,3,27,2),(252,1,11.00,3,27,3),(253,2,25.00,4,28,4),(254,1,0.00,2,29,4),(255,12,22.00,4,22,2),(256,3,22.00,4,22,4),(257,1,0.00,3,23,2),(258,1,0.00,3,23,5),(259,5,11.00,3,30,4),(260,1,11.00,3,24,2),(261,1,11.00,3,24,4),(262,1,24.00,4,31,4),(263,3,11.00,3,26,2),(264,1,11.00,3,26,4),(265,1,13.00,3,32,5),(266,1,0.00,4,33,3),(267,1,0.00,4,33,5),(268,3,14.00,3,34,3),(269,4,14.00,3,34,5),(270,2,11.00,3,35,3),(271,2,11.00,3,35,5),(272,1,20.00,37,36,17),(273,1,51.45,13,37,1),(277,1,19.45,38,39,1),(284,1,31.95,12,41,1),(285,1,19.95,39,42,1),(286,1,4.60,9,43,1),(287,1,6.95,39,44,1),(288,1,2.95,39,44,1),(289,8,2.95,39,45,1),(295,1,302.70,40,47,1),(296,1,14.95,39,48,1),(302,1,23.45,38,50,1),(321,50,1.34,13,49,1),(322,1,27.50,41,49,1),(323,1,30.00,38,49,1),(324,1,44.90,10,38,1),(325,1,13.70,40,38,1),(326,1,7.00,38,38,1),(327,1,4.30,8,40,1),(328,1,17.50,39,40,1),(329,1,13.35,42,46,1),(330,1,7.10,42,46,1),(331,2,16.30,42,46,1),(332,1,10.35,42,46,1),(333,1,8.45,42,46,1),(334,1,6.95,38,46,1),(335,1,12.60,42,51,1),(336,1,25.90,42,51,1),(337,2,4.65,39,51,1),(338,2,3.55,39,51,1),(339,1,6.95,38,51,1),(344,48,1.00,5,52,1),(345,28,0.95,5,52,1),(346,40,0.90,5,52,1),(347,1,38.00,41,52,1),(348,1,15.00,43,52,1),(349,1,10.00,43,52,1),(350,1,18.00,43,52,1),(351,1,21.00,43,52,1),(388,1,3.40,25,3,17),(389,1,3.40,26,3,17),(390,6,7.65,2,19,2),(391,4,7.65,2,19,4),(392,1,5.10,33,19,2),(393,1,5.10,33,19,5),(394,1,7.65,35,19,2),(395,1,7.70,35,19,2),(396,1,15.30,20,19,17),(397,1,3.40,26,19,17),(398,3,2.55,27,19,17),(399,3,5.10,28,19,17),(400,1,3.40,25,20,17),(401,1,3.40,26,20,17),(402,1,5.10,28,20,17),(403,1,16.00,39,53,1),(407,1,7.45,39,55,1),(408,2,3.25,39,56,1),(409,1,4.50,39,56,1),(410,1,3.50,39,56,1),(411,1,8.95,39,56,1),(412,2,5.95,39,54,1),(413,1,12.90,41,57,1),(414,1,1.36,14,57,1),(415,2,1.34,14,57,1),(416,1,7.75,14,57,1),(417,1,7.40,14,57,1),(418,1,5.41,14,57,1),(419,1,3.68,14,57,1),(420,1,61.02,38,57,1),(421,1,9.50,9,58,1),(422,1,26.20,9,58,1),(423,1,13.90,9,58,1),(424,1,6.00,21,9,17),(425,2,4.00,24,9,17),(426,5,4.00,25,9,17),(427,5,4.00,26,9,17),(428,1,3.00,27,9,17),(429,1,6.00,28,9,17),(430,1,6.00,30,9,17),(431,2,7.50,29,9,17),(432,6,13.00,3,9,6),(433,6,13.00,3,9,7),(434,6,13.00,3,9,8),(435,6,13.00,3,9,11),(436,7,7.00,2,9,8),(437,5,7.00,2,9,11),(438,1,9.00,34,9,8),(439,1,9.00,34,9,11),(440,3,4.50,1,9,6),(441,3,4.50,1,9,7),(442,2,4.50,1,9,8),(443,3,4.50,1,9,9),(444,4,4.50,1,9,10),(445,2,4.50,1,9,11),(446,1,4.50,1,9,12),(447,1,4.50,1,9,13),(448,1,4.50,1,9,14),(449,2,4.50,1,9,15),(450,1,5.66,32,9,12),(451,1,5.66,32,9,6),(452,1,5.66,32,9,7),(453,1,5.66,32,9,9),(454,1,5.66,1,9,11),(455,1,5.66,1,9,15),(456,8,6.50,31,9,16),(457,3,2.00,36,9,17),(458,1,4.25,35,9,8),(459,1,4.95,35,9,8),(461,1,14.95,44,59,1),(462,1,30.30,38,60,1),(463,1,2.95,37,61,1),(465,1,30.00,38,62,1),(471,40,0.86,5,63,1),(472,1,75.00,18,63,1),(473,1,15.00,18,63,1),(474,10,1.50,5,63,1),(475,10,2.90,5,63,1),(476,2,3.00,45,64,1),(477,75,0.10,45,64,1),(478,30,1.08,16,65,1),(483,2,14.00,17,67,1),(484,1,14.00,17,67,1),(487,2,3.00,45,69,1),(488,60,0.10,45,69,1),(489,1,89.90,18,66,1),(490,3,29.90,18,66,1),(491,1,75.00,46,68,1);
/*!40000 ALTER TABLE `receipts_products` ENABLE KEYS */;

-- 
-- Definition of suppliers
-- 

DROP TABLE IF EXISTS `suppliers`;
CREATE TABLE IF NOT EXISTS `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int NOT NULL,
  PRIMARY KEY (`supplier_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `suppliers_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table suppliers
-- 

/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers`(`supplier_id`,`fk_entity_id`) VALUES(11,1),(7,2),(13,9),(20,11),(18,12),(19,13),(1,16),(6,17),(4,18),(8,19),(2,20),(3,21),(5,22),(9,23),(10,24),(12,25),(15,26),(14,27),(16,28),(17,29),(21,30),(22,31),(23,32),(24,33),(25,34);
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;

-- 
-- Definition of receipts_suppliers
-- 

DROP TABLE IF EXISTS `receipts_suppliers`;
CREATE TABLE IF NOT EXISTS `receipts_suppliers` (
  `fk_receipt_id` int NOT NULL,
  `fk_supplier_id` int NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_supplier_id` (`fk_supplier_id`),
  CONSTRAINT `receipts_suppliers_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_suppliers_ibfk_2` FOREIGN KEY (`fk_supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_suppliers
-- 

/*!40000 ALTER TABLE `receipts_suppliers` DISABLE KEYS */;
INSERT INTO `receipts_suppliers`(`fk_receipt_id`,`fk_supplier_id`) VALUES(37,1),(39,3),(41,5),(42,6),(43,7),(44,8),(45,8),(47,10),(48,6),(50,3),(49,12),(38,2),(40,4),(46,9),(51,9),(52,13),(53,14),(55,6),(56,6),(54,15),(57,16),(58,17),(59,6),(60,3),(61,18),(62,19),(63,20),(64,21),(65,22),(67,24),(69,21),(66,23),(68,25);
/*!40000 ALTER TABLE `receipts_suppliers` ENABLE KEYS */;


/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2025-08-20 10:57:15
-- Total time: 0:0:0:0:192 (d:h:m:s:ms)
