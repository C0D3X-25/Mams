-- MySqlBackup.NET 2.6.5.0
-- Dump Time: 2026-04-14 14:37:42
-- --------------------------------------
-- Server version 11.4.5-MariaDB mariadb.org binary distribution


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of dose_units
-- 

DROP TABLE IF EXISTS `dose_units`;
CREATE TABLE IF NOT EXISTS `dose_units` (
  `dose_unit_id` int(11) NOT NULL AUTO_INCREMENT,
  `dose_unit_name` varchar(100) NOT NULL,
  `dose_unit_archive` date DEFAULT NULL,
  PRIMARY KEY (`dose_unit_id`),
  KEY `idx_dose_units_dose_unit_name` (`dose_unit_name`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table dose_units
-- 

/*!40000 ALTER TABLE `dose_units` DISABLE KEYS */;
INSERT INTO `dose_units`(`dose_unit_id`,`dose_unit_name`,`dose_unit_archive`) VALUES(1,'ml',NULL),(2,'bande',NULL);
/*!40000 ALTER TABLE `dose_units` ENABLE KEYS */;

-- 
-- Definition of entities
-- 

DROP TABLE IF EXISTS `entities`;
CREATE TABLE IF NOT EXISTS `entities` (
  `entity_id` int(11) NOT NULL AUTO_INCREMENT,
  `entity_name` varchar(50) NOT NULL,
  `entity_phone` varchar(25) DEFAULT NULL,
  `entity_email` varchar(255) DEFAULT NULL,
  `entity_city` varchar(50) DEFAULT NULL,
  `entity_address` varchar(255) DEFAULT NULL,
  `entity_archive` date DEFAULT NULL,
  PRIMARY KEY (`entity_id`),
  KEY `idx_entities_entity_name` (`entity_name`)
) ENGINE=InnoDB AUTO_INCREMENT=40 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table entities
-- 

/*!40000 ALTER TABLE `entities` DISABLE KEYS */;
INSERT INTO `entities`(`entity_id`,`entity_name`,`entity_phone`,`entity_email`,`entity_city`,`entity_address`,`entity_archive`) VALUES(1,'Bluette','','','','',NULL),(2,'Roggen','','','','',NULL),(3,'Ruchat','','','','',NULL),(4,'Espace Abeilles','','','','',NULL),(5,'Carmelina','','','','',NULL),(6,'Chandines','','','','',NULL),(7,'Arbothévoz','','','','',NULL),(8,'Particuliers','','','','',NULL),(9,'Rithner','','','','',NULL),(10,'Bienen Meier','','','','',NULL),(11,'Apimat','','','','',NULL),(12,'Landi','','','','',NULL),(13,'Marché Ressudens 10.05.25','','','','',NULL),(14,'Marco location','','','','',NULL),(15,'Nicolas location','','','','',NULL),(16,'Corso Ivrea','','','','','2025-10-04 00:00:00'),(17,'JUMBO','','','','',NULL),(18,'Bastella','','','','',NULL),(19,'MIGROS','','','','',NULL),(20,'TEMPONNEMOI.COM','','','','',NULL),(21,'Frais divers','','','','',NULL),(22,'smartphoto','','','','',NULL),(23,'VBS','','','','',NULL),(24,'L\'PIXL','026 675 53 90','info@lpixl.ch','1580 Avenches','Rue Bibracte 4a',NULL),(25,'ROUTE D\'OR F','','','','',NULL),(26,'MANOR','','','','',NULL),(27,'Rue des Arts sàrl','','','1870 Monthey','Rue de Coppet 2',NULL),(28,'Ocres de France','0033 4 90 74 63 82','www.ocres-de-france.com','F 84400 Apt','Ch des Ocriers 200',NULL),(29,'Droguerie du Portail','026 660 25 18','','1530 Payerne','Grand Rue 64',NULL),(30,'SAR','','','','',NULL),(31,'COOP','','','','',NULL),(32,'Schilliger','','','','',NULL),(33,'FVA','','','','',NULL),(34,'Kurt Nobs','079 252 69 52','','1583 Villarepos','La Solitude 5',NULL),(36,'La cartoleria ENRICO','+39 0125 61 58 83','www.enricocart.it','10015 Ivrea','Corso Vercelli 334',NULL),(37,'Sanima','026 305 22 82','www.sanima.ch','1725 Posieux','Rte de Grangeneuve 21',NULL),(38,'ASA-SR','','','','',NULL),(39,'D. Caso','','','','',NULL);
/*!40000 ALTER TABLE `entities` ENABLE KEYS */;

-- 
-- Definition of clients
-- 

DROP TABLE IF EXISTS `clients`;
CREATE TABLE IF NOT EXISTS `clients` (
  `client_id` int(11) NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int(11) NOT NULL,
  `client_archive` date DEFAULT NULL,
  PRIMARY KEY (`client_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `clients_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table clients
-- 

/*!40000 ALTER TABLE `clients` DISABLE KEYS */;
INSERT INTO `clients`(`client_id`,`fk_entity_id`,`client_archive`) VALUES(1,1,NULL),(5,2,NULL),(4,3,NULL),(3,4,NULL),(2,5,NULL),(7,6,NULL),(8,7,NULL),(11,8,NULL),(12,9,NULL),(6,13,NULL),(9,14,NULL),(10,15,NULL);
/*!40000 ALTER TABLE `clients` ENABLE KEYS */;

-- 
-- Definition of products_categories
-- 

DROP TABLE IF EXISTS `products_categories`;
CREATE TABLE IF NOT EXISTS `products_categories` (
  `product_category_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_category_name` varchar(100) NOT NULL,
  `product_category_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_category_id`),
  KEY `idx_products_categories_product_category_name` (`product_category_name`)
) ENGINE=InnoDB AUTO_INCREMENT=20 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_categories
-- 

/*!40000 ALTER TABLE `products_categories` DISABLE KEYS */;
INSERT INTO `products_categories`(`product_category_id`,`product_category_name`,`product_category_archive`) VALUES(1,'Miel',NULL),(2,'Bougie/cire',NULL),(3,'Savon',NULL),(4,'Divers',NULL),(10,'Matériel entretien',NULL),(11,'Matériel colonies',NULL),(12,'Administratif',NULL),(13,'Cire','2025-08-19 00:00:00'),(14,'Frais divers/port',NULL),(15,'Bien-être Apicultrice',NULL),(16,'Formation/cours',NULL),(18,'Publicité',NULL),(19,'Traitement',NULL);
/*!40000 ALTER TABLE `products_categories` ENABLE KEYS */;

-- 
-- Definition of products_shapes
-- 

DROP TABLE IF EXISTS `products_shapes`;
CREATE TABLE IF NOT EXISTS `products_shapes` (
  `product_shape_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_shape_name` varchar(100) NOT NULL,
  `product_shape_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_shape_id`),
  KEY `idx_products_shapes_product_shape_name` (`product_shape_name`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_name` varchar(100) NOT NULL,
  `product_weight` int(11) DEFAULT NULL,
  `product_archive` date DEFAULT NULL,
  `fk_product_type_id` int(11) NOT NULL,
  `fk_product_category_id` int(11) NOT NULL,
  `fk_product_shape_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`product_id`),
  KEY `fk_product_type_id` (`fk_product_type_id`),
  KEY `fk_product_category_id` (`fk_product_category_id`),
  KEY `fk_product_shape_id` (`fk_product_shape_id`),
  KEY `idx_products_product_name` (`product_name`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`fk_product_type_id`) REFERENCES `products_types` (`product_type_id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`fk_product_category_id`) REFERENCES `products_categories` (`product_category_id`),
  CONSTRAINT `products_ibfk_3` FOREIGN KEY (`fk_product_shape_id`) REFERENCES `products_shapes` (`product_shape_id`)
) ENGINE=InnoDB AUTO_INCREMENT=53 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products
-- 

/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products`(`product_id`,`product_name`,`product_weight`,`product_archive`,`fk_product_type_id`,`fk_product_category_id`,`fk_product_shape_id`) VALUES(1,'Miel 125g',125,NULL,1,1,1),(2,'Miel 250g',250,NULL,1,1,1),(3,'Miel 500g',500,NULL,1,1,1),(4,'Miel 1000g',1000,NULL,1,1,1),(5,'Bocaux, boîtes MeR',0,NULL,1,1,1),(6,'Etiquette',0,NULL,1,1,1),(7,'Moules bougies',0,NULL,1,2,1),(8,'Mèches bougies',0,NULL,1,2,1),(9,'Matière première',0,NULL,1,3,1),(10,'Tampon',0,NULL,1,3,1),(11,'Sacs',0,NULL,1,1,1),(12,'Carte/ visite',0,NULL,1,18,1),(13,'Emballage présentation',0,NULL,1,1,1),(14,'Peinture',0,NULL,2,10,1),(15,'Réparation',0,NULL,2,10,1),(16,'Nourissement',0,NULL,2,11,1),(17,'Oxuvar',0,NULL,2,19,1),(18,'Matériel apicole',0,NULL,2,11,1),(19,'Administratif',0,NULL,2,12,1),(20,'Bougie alvéoles grandes',0,NULL,1,2,1),(21,'Bougie alvéoles petites',0,NULL,1,2,1),(22,'Bougie 1001 Fleurs',0,NULL,1,2,1),(23,'Bougie longue',0,NULL,1,2,1),(24,'Bougie torsadée',0,NULL,1,2,1),(25,'Bougie ruche',0,NULL,1,2,1),(26,'Bougie hibou',0,NULL,1,2,1),(27,'Bougie florette',0,NULL,1,2,1),(28,'Bougie cire/bâton',0,NULL,1,2,1),(29,'Cire bricolage',0,NULL,1,2,1),(30,'Bougie cire gaufrée',0,NULL,1,2,1),(31,'API\'SAVON laurier',70,NULL,1,3,1),(32,'Miel trio 125',125,NULL,1,1,1),(33,'Miel duo 125',125,NULL,1,1,1),(34,'Miel duo 250',250,NULL,1,1,1),(35,'Miel en rayons 100g',100,NULL,1,1,1),(36,'Décorations cire',0,NULL,1,2,1),(37,'Cire brut',100,NULL,1,2,1),(38,'Taxes/douane/livraison',0,NULL,1,14,1),(39,'Matériel décorations',0,NULL,1,4,1),(40,'Logo CT',0,NULL,1,18,1),(41,'Livres',0,NULL,2,16,1),(42,'Moules décos cire',0,NULL,1,2,1),(43,'Soins santé',0,NULL,2,15,1),(44,'Bâton créatif',0,NULL,1,2,1),(45,'Contrôle miel',0,NULL,1,1,1),(46,'Reine Sélection',0,NULL,2,11,1),(47,'Miel en rayons 200g',200,NULL,1,1,1),(48,'Miel en rayons 500g',500,NULL,1,1,1),(49,'Miel en rayons 600g',600,NULL,1,1,1),(50,'Miel en rayons 400g',400,NULL,1,1,1),(51,'Formivar 70%',0,NULL,2,19,1),(52,'FormicPro',0,NULL,2,19,1);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;

-- 
-- Definition of products_types
-- 

DROP TABLE IF EXISTS `products_types`;
CREATE TABLE IF NOT EXISTS `products_types` (
  `product_type_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_type_name` varchar(100) NOT NULL,
  `product_type_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_type_id`),
  KEY `idx_products_types_product_type_name` (`product_type_name`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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
  `receipt_id` int(11) NOT NULL AUTO_INCREMENT,
  `receipt_number` varchar(50) NOT NULL,
  `receipt_total_price` decimal(9,2) NOT NULL,
  `receipt_date_created` date NOT NULL,
  PRIMARY KEY (`receipt_id`),
  KEY `idx_receipts_receipt_number` (`receipt_number`),
  KEY `idx_receipts_receipt_total_price` (`receipt_total_price`),
  KEY `idx_receipts_receipt_date_created` (`receipt_date_created`)
) ENGINE=InnoDB AUTO_INCREMENT=103 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts
-- 

/*!40000 ALTER TABLE `receipts` DISABLE KEYS */;
INSERT INTO `receipts`(`receipt_id`,`receipt_number`,`receipt_total_price`,`receipt_date_created`) VALUES(1,'74-25',101.00,'2025-06-11 00:00:00'),(2,'73-25',142.50,'2025-05-29 00:00:00'),(3,'EA0125',6.80,'2025-05-12 00:00:00'),(4,'71-25',87.00,'2025-05-28 00:00:00'),(5,'72-25',322.50,'2025-05-28 00:00:00'),(9,'MarR25',698.16,'2025-05-10 00:00:00'),(10,'EA0525',105.45,'2025-09-12 00:00:00'),(11,'75-25',285.00,'2025-07-29 00:00:00'),(12,'76-25',116.00,'2025-07-31 00:00:00'),(13,'AT125',0.00,'2025-06-04 00:00:00'),(14,'AT225',0.00,'2025-08-11 00:00:00'),(15,'ML225',0.00,'2025-07-30 00:00:00'),(16,'ML125',0.00,'2025-05-29 00:00:00'),(17,'NL125',0.00,'2025-06-11 00:00:00'),(18,'NL225',0.00,'2025-07-31 00:00:00'),(19,'EA0325',143.70,'2025-08-08 00:00:00'),(20,'EA0225',11.90,'2025-06-16 00:00:00'),(21,'P30525',78.00,'2025-05-30 00:00:00'),(22,'P40525',330.00,'2025-05-31 00:00:00'),(23,'P10625',0.00,'2025-06-01 00:00:00'),(24,'P30625',22.00,'2025-06-20 00:00:00'),(25,'P10725',85.00,'2025-07-01 00:00:00'),(26,'P20725',44.00,'2025-07-21 00:00:00'),(27,'P20825',22.00,'2025-08-12 00:00:00'),(28,'P10525',50.00,'2025-05-23 00:00:00'),(29,'P20525',0.00,'2025-05-24 00:00:00'),(30,'P20625',55.00,'2025-06-04 00:00:00'),(31,'P40625',24.00,'2025-06-27 00:00:00'),(32,'P30725',13.00,'2025-07-21 00:00:00'),(33,'P40725',0.00,'2025-07-27 00:00:00'),(34,'P10825',98.00,'2025-08-06 00:00:00'),(35,'P30825',44.00,'2025-08-12 00:00:00'),(36,'CB0125',19.80,'2025-03-27 00:00:00'),(37,'244',51.45,'2025-01-20 00:00:00'),(38,'245',65.60,'2025-01-24 00:00:00'),(39,'246',19.45,'2025-01-30 00:00:00'),(40,'247',21.80,'2025-02-01 00:00:00'),(41,'248',31.95,'2025-02-04 00:00:00'),(42,'249',19.95,'2025-02-13 00:00:00'),(43,'249B',4.60,'2025-02-13 00:00:00'),(44,'250',9.90,'2025-02-13 00:00:00'),(45,'251',23.60,'2025-02-14 00:00:00'),(46,'252',78.80,'2025-02-27 00:00:00'),(47,'253',302.70,'2025-02-28 00:00:00'),(48,'254',14.95,'2025-03-06 00:00:00'),(49,'255',124.50,'2025-03-11 00:00:00'),(50,'256',23.45,'2025-03-17 00:00:00'),(51,'257',61.85,'2025-03-27 00:00:00'),(52,'258',212.60,'2025-03-27 00:00:00'),(53,'259',16.00,'2025-03-27 00:00:00'),(54,'258B',11.90,'2025-03-27 00:00:00'),(55,'260',7.45,'2025-03-27 00:00:00'),(56,'261',23.45,'2025-03-29 00:00:00'),(57,'262',102.20,'2025-03-29 00:00:00'),(58,'263',49.60,'2025-04-10 00:00:00'),(59,'264',14.95,'2025-04-30 00:00:00'),(60,'265',30.30,'2025-05-02 00:00:00'),(61,'266',2.95,'2025-05-01 00:00:00'),(62,'267',30.00,'2025-05-10 00:00:00'),(63,'268',168.40,'2025-05-24 00:00:00'),(64,'269',13.50,'2025-05-26 00:00:00'),(65,'270',32.40,'2025-06-02 00:00:00'),(66,'271',179.60,'2025-06-09 00:00:00'),(67,'272',42.00,'2025-06-18 00:00:00'),(68,'273',75.00,'2025-07-11 00:00:00'),(69,'274',12.00,'2025-07-28 00:00:00'),(70,'EA0425',35.70,'2025-08-29 00:00:00'),(71,'P50825',0.00,'2025-08-30 00:00:00'),(72,'P10925',48.00,'2025-09-08 00:00:00'),(73,'P20925',0.00,'2025-09-27 00:00:00'),(74,'275',32.40,'2025-08-08 00:00:00'),(75,'276',21.00,'2025-08-26 00:00:00'),(76,'277',21.00,'2025-09-08 00:00:00'),(77,'278',64.70,'2025-09-20 00:00:00'),(78,'279',24.90,'2025-09-27 00:00:00'),(79,'77-25',136.00,'2025-10-08 00:00:00'),(80,'P11025',0.00,'2025-10-23 00:00:00'),(81,'280',14.95,'2025-10-20 00:00:00'),(82,'281',16.90,'2025-10-29 00:00:00'),(83,'282',86.00,'2025-10-30 00:00:00'),(84,'P40825',11.00,'2025-08-20 00:00:00'),(85,'EA0625',106.50,'2025-11-10 00:00:00'),(86,'78-25',90.00,'2025-11-13 00:00:00'),(87,'79-25',126.00,'2025-11-15 00:00:00'),(88,'P11125',0.00,'2025-11-16 00:00:00'),(89,'P21125',40.00,'2025-11-29 00:00:00'),(90,'P11225',22.00,'2025-12-18 00:00:00'),(91,'283',87.00,'2025-12-29 00:00:00'),(92,'EA0725',5.10,'2025-12-30 00:00:00'),(93,'284',24.80,'2026-01-05 00:00:00'),(94,'P10226',44.00,'2026-02-20 00:00:00'),(95,'285',84.00,'2026-01-17 00:00:00'),(96,'P20226',7.00,'2026-02-24 00:00:00'),(97,'286',131.60,'2026-02-24 00:00:00');
/*!40000 ALTER TABLE `receipts` ENABLE KEYS */;

-- 
-- Definition of receipts_clients
-- 

DROP TABLE IF EXISTS `receipts_clients`;
CREATE TABLE IF NOT EXISTS `receipts_clients` (
  `fk_receipt_id` int(11) NOT NULL,
  `fk_client_id` int(11) NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_client_id` (`fk_client_id`),
  CONSTRAINT `receipts_clients_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_clients_ibfk_2` FOREIGN KEY (`fk_client_id`) REFERENCES `clients` (`client_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_clients
-- 

/*!40000 ALTER TABLE `receipts_clients` DISABLE KEYS */;
INSERT INTO `receipts_clients`(`fk_receipt_id`,`fk_client_id`) VALUES(3,3),(28,11),(29,11),(21,11),(22,11),(23,11),(30,11),(24,11),(31,11),(25,11),(26,11),(32,11),(34,11),(27,11),(35,11),(16,9),(15,9),(17,10),(18,10),(13,8),(14,8),(4,4),(5,5),(2,2),(1,1),(11,7),(12,1),(79,4),(20,3),(80,11),(84,11),(71,11),(86,5),(87,7),(72,11),(10,3),(73,11),(85,3),(19,3),(70,3),(88,11),(89,11),(33,11),(90,11),(92,3),(94,11),(9,6),(36,12),(96,11);
/*!40000 ALTER TABLE `receipts_clients` ENABLE KEYS */;

-- 
-- Definition of regions
-- 

DROP TABLE IF EXISTS `regions`;
CREATE TABLE IF NOT EXISTS `regions` (
  `region_id` int(11) NOT NULL AUTO_INCREMENT,
  `region_name` varchar(100) NOT NULL,
  `region_archive` date DEFAULT NULL,
  PRIMARY KEY (`region_id`),
  KEY `idx_regions_region_name` (`region_name`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table regions
-- 

/*!40000 ALTER TABLE `regions` DISABLE KEYS */;
INSERT INTO `regions`(`region_id`,`region_name`,`region_archive`) VALUES(1,'','1901-01-01 00:00:00'),(2,'Argovie',NULL),(3,'Appenzell Rhodes-Intérieures',NULL),(4,'Appenzell Rhodes-Extérieures',NULL),(5,'Berne',NULL),(6,'Bâle-Campagne',NULL),(7,'Bâle-Ville',NULL),(8,'Fribourg',NULL),(9,'Genève',NULL),(10,'Glaris',NULL),(11,'Grisons',NULL),(12,'Jura',NULL),(13,'Lucerne',NULL),(14,'Neuchâtel',NULL),(15,'Nidwald',NULL),(16,'Obwald',NULL),(17,'Saint-Gall',NULL),(18,'Schaffhouse',NULL),(19,'Soleure',NULL),(20,'Schwyz',NULL),(21,'Thurgovie',NULL),(22,'Tessin',NULL),(23,'Uri',NULL),(24,'Vaud',NULL),(25,'Valais',NULL),(26,'Zoug',NULL),(27,'Zurich',NULL);
/*!40000 ALTER TABLE `regions` ENABLE KEYS */;

-- 
-- Definition of beehives
-- 

DROP TABLE IF EXISTS `beehives`;
CREATE TABLE IF NOT EXISTS `beehives` (
  `beehive_id` int(11) NOT NULL AUTO_INCREMENT,
  `beehive_name` varchar(50) NOT NULL,
  `beehive_number` varchar(50) DEFAULT NULL,
  `beehive_archive` date DEFAULT NULL,
  `fk_region_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`beehive_id`),
  KEY `idx_beehives_beehive_name` (`beehive_name`),
  KEY `fk_region_id` (`fk_region_id`),
  CONSTRAINT `beehives_ibfk_1` FOREIGN KEY (`fk_region_id`) REFERENCES `regions` (`region_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table beehives
-- 

/*!40000 ALTER TABLE `beehives` DISABLE KEYS */;
INSERT INTO `beehives`(`beehive_id`,`beehive_name`,`beehive_number`,`beehive_archive`,`fk_region_id`) VALUES(1,'',NULL,'2026-04-14 00:00:00',1),(2,'La Côte','VD 58175006',NULL,24),(3,'Le Laret','FR 215307',NULL,8),(4,'Petit Belmont','FR 205326',NULL,8);
/*!40000 ALTER TABLE `beehives` ENABLE KEYS */;

-- 
-- Definition of products_lots
-- 

DROP TABLE IF EXISTS `products_lots`;
CREATE TABLE IF NOT EXISTS `products_lots` (
  `product_lot_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_lot_name` varchar(50) NOT NULL,
  `product_lot_year` int(11) NOT NULL,
  `product_lot_archive` date DEFAULT NULL,
  `fk_beehive_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`product_lot_id`),
  KEY `fk_beehive_id` (`fk_beehive_id`),
  KEY `idx_products_lots_product_lot_name` (`product_lot_name`),
  KEY `idx_products_lots_product_lot_year` (`product_lot_year`),
  CONSTRAINT `products_lots_ibfk_1` FOREIGN KEY (`fk_beehive_id`) REFERENCES `beehives` (`beehive_id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table products_lots
-- 

/*!40000 ALTER TABLE `products_lots` DISABLE KEYS */;
INSERT INTO `products_lots`(`product_lot_id`,`product_lot_name`,`product_lot_year`,`product_lot_archive`,`fk_beehive_id`) VALUES(1,'',0,'1901-01-01 00:00:00',1),(2,'LL125',2025,NULL,3),(3,'LL225',2025,NULL,3),(4,'LC125',2025,NULL,2),(5,'LC225',2025,NULL,2),(6,'L124',2024,NULL,3),(7,'L224',2024,NULL,3),(8,'L324',2024,NULL,3),(9,'L241',2024,NULL,2),(10,'L242',2024,NULL,2),(11,'L243',2024,NULL,2),(12,'L123',2023,NULL,3),(13,'L223',2023,NULL,3),(14,'L231',2023,NULL,2),(15,'L232',2023,NULL,2),(16,'L1.25',2025,NULL,3),(17,'LCCR25',2025,NULL,2),(18,'LLCR25',2025,NULL,3);
/*!40000 ALTER TABLE `products_lots` ENABLE KEYS */;

-- 
-- Definition of receipts_products
-- 

DROP TABLE IF EXISTS `receipts_products`;
CREATE TABLE IF NOT EXISTS `receipts_products` (
  `receipt_product_id` int(11) NOT NULL AUTO_INCREMENT,
  `receipt_product_quantity` int(11) NOT NULL,
  `receipt_product_unity_price` decimal(9,2) NOT NULL,
  `fk_product_id` int(11) NOT NULL,
  `fk_receipt_id` int(11) NOT NULL,
  `fk_product_lot_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`receipt_product_id`),
  KEY `fk_product_id` (`fk_product_id`),
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_product_lot_id` (`fk_product_lot_id`),
  KEY `idx_receipts_products_receipt_product_quantity` (`receipt_product_quantity`),
  KEY `idx_receipts_products_receipt_product_unity_price` (`receipt_product_unity_price`),
  CONSTRAINT `receipts_products_ibfk_1` FOREIGN KEY (`fk_product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `receipts_products_ibfk_2` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_products_ibfk_3` FOREIGN KEY (`fk_product_lot_id`) REFERENCES `products_lots` (`product_lot_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1112 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_products
-- 

/*!40000 ALTER TABLE `receipts_products` DISABLE KEYS */;
INSERT INTO `receipts_products`(`receipt_product_id`,`receipt_product_quantity`,`receipt_product_unity_price`,`fk_product_id`,`fk_receipt_id`,`fk_product_lot_id`) VALUES(510,1,3.40,25,3,17),(511,1,3.40,26,3,17),(518,2,25.00,4,28,4),(519,1,0.00,2,29,4),(520,6,13.00,3,21,2),(521,12,22.00,4,22,2),(522,3,22.00,4,22,4),(523,1,0.00,3,23,2),(524,1,0.00,3,23,5),(525,5,11.00,3,30,4),(526,1,11.00,3,24,2),(527,1,11.00,3,24,4),(528,1,24.00,4,31,4),(529,5,11.00,3,25,2),(530,5,6.00,2,25,2),(531,3,11.00,3,26,2),(532,1,11.00,3,26,4),(533,1,13.00,3,32,5),(536,3,14.00,3,34,3),(537,4,14.00,3,34,5),(538,1,11.00,3,27,2),(539,1,11.00,3,27,3),(540,2,11.00,3,35,3),(541,2,11.00,3,35,5),(543,2,0.00,3,16,4),(544,3,0.00,3,15,5),(545,3,0.00,3,17,2),(546,4,0.00,3,18,3),(547,10,0.00,3,13,4),(548,5,0.00,3,14,5),(586,6,9.50,3,4,2),(587,6,5.00,2,4,2),(588,10,18.00,4,5,2),(589,15,9.50,3,5,2),(590,15,9.50,3,2,4),(591,8,9.50,3,1,2),(592,5,5.00,2,1,2),(593,5,9.50,3,11,4),(594,25,9.50,3,11,5),(595,8,9.50,3,12,3),(596,3,5.00,2,12,2),(597,5,5.00,2,12,3),(681,1,51.45,13,37,1),(682,1,44.90,10,38,1),(683,1,13.70,40,38,1),(684,1,7.00,38,38,1),(685,1,19.45,38,39,1),(686,1,4.30,8,40,1),(687,1,17.50,39,40,1),(690,1,4.60,9,43,1),(691,1,6.95,39,44,1),(692,1,2.95,39,44,1),(693,8,2.95,39,45,1),(694,1,13.35,42,46,1),(695,1,7.10,42,46,1),(696,2,16.30,42,46,1),(697,1,10.35,42,46,1),(698,1,8.45,42,46,1),(699,1,6.95,38,46,1),(700,1,302.70,40,47,1),(702,50,1.34,13,49,1),(703,1,27.50,41,49,1),(704,1,30.00,38,49,1),(705,1,23.45,38,50,1),(706,1,12.60,42,51,1),(707,1,25.90,42,51,1),(708,2,4.65,39,51,1),(709,2,3.55,39,51,1),(710,1,6.95,38,51,1),(711,48,1.00,5,52,1),(712,28,0.95,5,52,1),(713,40,0.90,5,52,1),(714,1,38.00,41,52,1),(715,1,15.00,43,52,1),(716,1,10.00,43,52,1),(717,1,18.00,43,52,1),(718,1,21.00,43,52,1),(719,1,16.00,39,53,1),(734,2,5.95,39,54,1),(735,1,7.45,39,55,1),(736,2,3.25,39,56,1),(737,1,4.50,39,56,1),(738,1,3.50,39,56,1),(739,1,8.95,39,56,1),(740,1,12.90,41,57,1),(741,1,1.36,14,57,1),(742,2,1.34,14,57,1),(743,1,7.75,14,57,1),(744,1,7.40,14,57,1),(745,1,5.41,14,57,1),(746,1,3.68,14,57,1),(747,1,61.02,38,57,1),(748,1,9.50,9,58,1),(749,1,26.20,9,58,1),(750,1,13.90,9,58,1),(751,1,14.95,44,59,1),(752,1,2.95,37,61,1),(753,1,30.30,38,60,1),(754,1,30.00,38,62,1),(760,2,3.00,45,64,1),(761,75,0.10,45,64,1),(762,30,1.08,16,65,1),(763,1,89.90,18,66,1),(764,3,29.90,18,66,1),(765,2,14.00,51,67,1),(766,1,14.00,17,67,1),(767,1,75.00,46,68,1),(768,2,3.00,45,69,1),(769,60,0.10,45,69,1),(770,30,1.08,16,74,1),(772,7,3.00,19,75,1),(773,20,1.05,16,76,1),(774,1,64.70,6,77,1),(775,1,24.90,11,78,1),(776,1,14.95,44,48,1),(777,8,9.50,3,79,3),(778,12,5.00,2,79,3),(779,40,0.86,5,63,1),(780,1,75.00,18,63,1),(781,1,15.00,18,63,1),(782,10,1.50,5,63,1),(783,10,2.90,5,63,1),(784,1,19.95,39,42,1),(785,1,31.95,12,41,1),(852,1,3.40,25,20,17),(853,1,3.40,26,20,17),(854,1,5.10,28,20,17),(928,1,0.00,2,80,5),(929,1,14.95,44,81,1),(930,1,14.95,44,82,1),(931,1,1.95,8,82,1),(932,1,86.00,19,83,1),(939,1,11.00,3,84,5),(940,1,0.00,3,71,4),(957,5,18.00,4,86,3),(959,10,5.00,2,87,5),(960,8,9.50,3,87,5),(961,1,24.00,4,72,2),(962,1,24.00,4,72,5),(983,3,5.10,32,10,2),(984,3,5.10,32,10,3),(985,3,5.10,32,10,5),(986,1,5.10,33,10,3),(987,1,5.10,33,10,5),(988,3,0.85,36,10,18),(989,1,7.20,47,10,2),(990,1,8.25,47,10,2),(991,1,8.65,47,10,2),(992,1,22.70,49,10,2),(993,1,0.00,32,73,2),(994,1,0.00,32,73,3),(995,1,0.00,32,73,5),(996,1,0.00,36,73,18),(997,3,5.10,21,85,18),(998,2,3.40,23,85,18),(999,1,2.55,27,85,18),(1000,1,17.90,50,85,2),(1001,1,20.75,48,85,2),(1002,1,22.80,49,85,2),(1003,2,5.10,33,85,3),(1004,2,5.10,33,85,5),(1005,6,7.65,2,19,2),(1006,4,7.65,2,19,4),(1007,1,5.10,33,19,2),(1008,1,5.10,33,19,4),(1009,1,7.65,47,19,2),(1010,1,7.70,47,19,2),(1011,1,15.30,20,19,18),(1012,1,3.40,26,19,18),(1013,3,2.55,27,19,18),(1014,3,5.10,28,19,18),(1015,1,5.10,21,70,18),(1016,3,5.10,33,70,2),(1017,3,5.10,33,70,4),(1019,1,0.00,26,88,18),(1020,4,10.00,3,89,5),(1021,1,0.00,4,33,3),(1022,1,0.00,4,33,5),(1023,1,22.00,4,90,5),(1034,1,5.10,21,92,18),(1040,8,1.85,12,93,1),(1041,10,1.00,12,93,1),(1042,1,22.00,7,91,1),(1043,1,23.00,7,91,1),(1044,1,29.00,7,91,1),(1045,1,13.00,38,91,1),(1053,2,31.00,43,95,1),(1054,1,22.00,43,95,1),(1055,2,22.00,4,94,5),(1056,1,6.00,21,9,17),(1057,2,4.00,24,9,17),(1058,5,4.00,25,9,17),(1059,5,4.00,26,9,17),(1060,1,3.00,27,9,17),(1061,1,6.00,28,9,17),(1062,1,6.00,30,9,17),(1063,2,7.50,29,9,17),(1064,6,13.00,3,9,6),(1065,6,13.00,3,9,7),(1066,6,13.00,3,9,8),(1067,6,13.00,3,9,11),(1068,7,7.00,2,9,8),(1069,5,7.00,2,9,11),(1070,1,9.00,34,9,8),(1071,1,9.00,34,9,11),(1072,3,4.50,1,9,6),(1073,3,4.50,1,9,7),(1074,2,4.50,1,9,8),(1075,3,4.50,1,9,9),(1076,4,4.50,1,9,10),(1077,2,4.50,1,9,11),(1078,1,4.50,1,9,12),(1079,1,4.50,1,9,13),(1080,1,4.50,1,9,14),(1081,2,4.50,1,9,15),(1082,1,5.66,32,9,12),(1083,1,5.66,32,9,6),(1084,1,5.66,32,9,7),(1085,1,5.66,32,9,9),(1086,1,5.66,1,9,11),(1087,1,5.66,1,9,15),(1088,8,6.50,31,9,16),(1089,3,2.00,36,9,17),(1090,1,4.25,35,9,8),(1091,1,4.95,35,9,8),(1092,22,0.90,37,36,17),(1093,1,7.00,2,96,3),(1100,20,1.20,5,97,1),(1101,48,1.00,5,97,1),(1102,28,0.95,5,97,1),(1103,2,10.00,43,97,1),(1104,1,13.00,43,97,1);
/*!40000 ALTER TABLE `receipts_products` ENABLE KEYS */;

-- 
-- Definition of suppliers
-- 

DROP TABLE IF EXISTS `suppliers`;
CREATE TABLE IF NOT EXISTS `suppliers` (
  `supplier_id` int(11) NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int(11) NOT NULL,
  `supplier_archive` date DEFAULT NULL,
  PRIMARY KEY (`supplier_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `suppliers_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=30 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table suppliers
-- 

/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers`(`supplier_id`,`fk_entity_id`,`supplier_archive`) VALUES(11,1,NULL),(7,2,NULL),(13,9,NULL),(20,11,NULL),(18,12,NULL),(19,13,NULL),(1,16,NULL),(6,17,NULL),(4,18,NULL),(8,19,NULL),(2,20,NULL),(3,21,NULL),(5,22,NULL),(9,23,NULL),(10,24,NULL),(12,25,NULL),(15,26,NULL),(14,27,NULL),(16,28,NULL),(17,29,NULL),(21,30,NULL),(22,31,NULL),(23,32,NULL),(24,33,NULL),(25,34,NULL),(26,36,NULL),(27,37,NULL),(28,38,NULL),(29,39,NULL);
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;

-- 
-- Definition of receipts_suppliers
-- 

DROP TABLE IF EXISTS `receipts_suppliers`;
CREATE TABLE IF NOT EXISTS `receipts_suppliers` (
  `fk_receipt_id` int(11) NOT NULL,
  `fk_supplier_id` int(11) NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_supplier_id` (`fk_supplier_id`),
  CONSTRAINT `receipts_suppliers_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_suppliers_ibfk_2` FOREIGN KEY (`fk_supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table receipts_suppliers
-- 

/*!40000 ALTER TABLE `receipts_suppliers` DISABLE KEYS */;
INSERT INTO `receipts_suppliers`(`fk_receipt_id`,`fk_supplier_id`) VALUES(37,26),(38,2),(39,3),(40,4),(43,7),(44,8),(45,8),(46,9),(47,10),(49,12),(50,3),(51,9),(52,13),(53,14),(54,15),(55,6),(56,6),(57,16),(58,17),(59,6),(61,18),(60,3),(62,19),(64,21),(65,22),(66,23),(67,24),(68,25),(69,21),(74,22),(75,27),(76,22),(77,5),(78,26),(48,6),(63,20),(42,6),(41,5),(81,6),(82,6),(83,21),(93,5),(91,20),(95,28),(97,13);
/*!40000 ALTER TABLE `receipts_suppliers` ENABLE KEYS */;

-- 
-- Definition of treatment_stocks
-- 

DROP TABLE IF EXISTS `treatment_stocks`;
CREATE TABLE IF NOT EXISTS `treatment_stocks` (
  `treatment_stock_id` int(11) NOT NULL AUTO_INCREMENT,
  `treatment_stock_purchase_date` date NOT NULL,
  `treatment_stock_initial_quantity` decimal(9,2) NOT NULL,
  `treatment_stock_first_used_date` date DEFAULT NULL,
  `treatment_stock_last_used_date` date DEFAULT NULL,
  `fk_product_id` int(11) NOT NULL,
  `fk_dose_unit_id` int(11) DEFAULT NULL,
  `fk_supplier_id` int(11) DEFAULT NULL,
  PRIMARY KEY (`treatment_stock_id`),
  KEY `fk_product_id` (`fk_product_id`),
  KEY `fk_dose_unit_id` (`fk_dose_unit_id`),
  KEY `fk_supplier_id` (`fk_supplier_id`),
  CONSTRAINT `treatment_stocks_ibfk_1` FOREIGN KEY (`fk_product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `treatment_stocks_ibfk_2` FOREIGN KEY (`fk_dose_unit_id`) REFERENCES `dose_units` (`dose_unit_id`),
  CONSTRAINT `treatment_stocks_ibfk_3` FOREIGN KEY (`fk_supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table treatment_stocks
-- 

/*!40000 ALTER TABLE `treatment_stocks` DISABLE KEYS */;
INSERT INTO `treatment_stocks`(`treatment_stock_id`,`treatment_stock_purchase_date`,`treatment_stock_initial_quantity`,`treatment_stock_first_used_date`,`treatment_stock_last_used_date`,`fk_product_id`,`fk_dose_unit_id`,`fk_supplier_id`) VALUES(1,'2021-01-01 00:00:00',1800.00,'2021-09-05 00:00:00','2021-09-21 00:00:00',51,1,29),(2,'2021-01-01 00:00:00',440.00,'2021-12-18 00:00:00','2021-12-30 00:00:00',17,1,29),(3,'2022-01-01 00:00:00',2930.00,'2022-08-04 00:00:00','2022-09-26 00:00:00',51,1,29),(4,'2022-01-01 00:00:00',350.00,'2022-12-21 00:00:00','2022-12-22 00:00:00',17,1,29),(5,'2023-01-01 00:00:00',3180.00,'2023-07-28 00:00:00','2023-09-28 00:00:00',51,1,29),(6,'2023-06-21 00:00:00',500.00,'2023-12-26 00:00:00','2023-12-27 00:00:00',17,1,29),(7,'2024-06-19 00:00:00',550.00,'2025-01-02 00:00:00','2025-01-05 00:00:00',17,1,29),(8,'2024-06-19 00:00:00',2000.00,'2024-09-20 00:00:00',NULL,51,1,29),(9,'2024-06-19 00:00:00',21.00,'2024-08-19 00:00:00','2024-10-11 00:00:00',52,2,29),(10,'2025-06-18 00:00:00',275.00,NULL,NULL,17,1,29),(11,'2025-06-18 00:00:00',2000.00,'2025-08-22 00:00:00',NULL,51,1,29);
/*!40000 ALTER TABLE `treatment_stocks` ENABLE KEYS */;

-- 
-- Definition of treatments
-- 

DROP TABLE IF EXISTS `treatments`;
CREATE TABLE IF NOT EXISTS `treatments` (
  `treatment_id` int(11) NOT NULL AUTO_INCREMENT,
  `treatment_date` date NOT NULL,
  `treatment_hive_count` int(11) NOT NULL,
  `treatment_dose_per_hive` decimal(9,2) NOT NULL,
  `fk_beehive_id` int(11) NOT NULL,
  `fk_treatment_stock_id` int(11) NOT NULL,
  PRIMARY KEY (`treatment_id`),
  KEY `fk_beehive_id` (`fk_beehive_id`),
  KEY `fk_treatment_stock_id` (`fk_treatment_stock_id`),
  KEY `idx_treatments_treatment_date` (`treatment_date`),
  CONSTRAINT `treatments_ibfk_1` FOREIGN KEY (`fk_beehive_id`) REFERENCES `beehives` (`beehive_id`),
  CONSTRAINT `treatments_ibfk_2` FOREIGN KEY (`fk_treatment_stock_id`) REFERENCES `treatment_stocks` (`treatment_stock_id`)
) ENGINE=InnoDB AUTO_INCREMENT=29 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table treatments
-- 

/*!40000 ALTER TABLE `treatments` DISABLE KEYS */;
INSERT INTO `treatments`(`treatment_id`,`treatment_date`,`treatment_hive_count`,`treatment_dose_per_hive`,`fk_beehive_id`,`fk_treatment_stock_id`) VALUES(1,'2021-09-05 00:00:00',8,126.25,2,1),(2,'2021-09-21 00:00:00',7,112.86,3,1),(3,'2021-12-18 00:00:00',7,32.86,2,2),(4,'2021-12-30 00:00:00',6,35.00,3,2),(5,'2022-08-04 00:00:00',6,80.00,2,3),(6,'2022-08-17 00:00:00',5,130.00,3,3),(7,'2022-09-11 00:00:00',8,127.50,2,3),(8,'2022-09-26 00:00:00',6,130.00,3,3),(9,'2022-12-21 00:00:00',7,35.71,2,4),(10,'2022-12-22 00:00:00',3,33.33,3,4),(11,'2023-07-28 00:00:00',5,130.00,3,5),(12,'2023-08-08 00:00:00',6,80.00,2,5),(13,'2023-09-11 00:00:00',8,128.75,3,5),(14,'2023-09-28 00:00:00',8,127.50,2,5),(15,'2023-12-26 00:00:00',8,28.75,3,6),(16,'2023-12-27 00:00:00',8,33.75,2,6),(17,'2024-08-19 00:00:00',4,2.00,2,9),(18,'2024-09-04 00:00:00',6,2.00,3,9),(19,'2024-09-20 00:00:00',8,127.50,2,8),(20,'2024-10-10 00:00:00',5,130.00,3,8),(21,'2024-10-10 00:00:00',1,0.50,3,9),(22,'2024-10-11 00:00:00',1,0.50,4,9),(23,'2025-01-02 00:00:00',6,41.67,2,7),(24,'2025-01-05 00:00:00',6,45.00,3,7),(25,'2025-07-15 00:00:00',2,130.00,3,8),(26,'2025-08-22 00:00:00',5,130.00,2,11),(27,'2025-09-07 00:00:00',4,130.00,3,11),(28,'2025-10-05 00:00:00',7,128.57,2,11);
/*!40000 ALTER TABLE `treatments` ENABLE KEYS */;

-- 
-- Definition of users
-- 

DROP TABLE IF EXISTS `users`;
CREATE TABLE IF NOT EXISTS `users` (
  `user_id` int(11) NOT NULL AUTO_INCREMENT,
  `user_name` varchar(50) NOT NULL,
  `user_phone` varchar(25) DEFAULT NULL,
  `user_email` varchar(255) DEFAULT NULL,
  `user_city` varchar(50) DEFAULT NULL,
  `user_address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table users
-- 

/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users`(`user_id`,`user_name`,`user_phone`,`user_email`,`user_city`,`user_address`) VALUES(1,'Corinne Thumelin','026 667 11 78','','1773 Russy','Rte de l\'Ecole 12');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;


/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;
/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2026-04-14 14:37:42
-- Total time: 0:0:0:0:230 (d:h:m:s:ms)
