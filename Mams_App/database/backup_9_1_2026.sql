-- MySQL dump 10.13  Distrib 8.0.42, for Win64 (x86_64)
--
-- Host: localhost    Database: mams_db
-- ------------------------------------------------------
-- Server version	8.0.42

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `regions`
--

DROP TABLE IF EXISTS `regions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `regions` (
  `region_id` int NOT NULL AUTO_INCREMENT,
  `region_name` varchar(100) NOT NULL,
  `region_archive` date DEFAULT NULL,
  PRIMARY KEY (`region_id`),
  KEY `idx_region_name` (`region_name`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `regions`
--

LOCK TABLES `regions` WRITE;
/*!40000 ALTER TABLE `regions` DISABLE KEYS */;
INSERT INTO `regions` VALUES (1,'','1901-01-01'),(2,'Argovie',NULL),(3,'Appenzell Rhodes-Intérieures',NULL),(4,'Appenzell Rhodes-Extérieures',NULL),(5,'Berne',NULL),(6,'Bâle-Campagne',NULL),(7,'Bâle-Ville',NULL),(8,'Fribourg',NULL),(9,'Genève',NULL),(10,'Glaris',NULL),(11,'Grisons',NULL),(12,'Jura',NULL),(13,'Lucerne',NULL),(14,'Neuchâtel',NULL),(15,'Nidwald',NULL),(16,'Obwald',NULL),(17,'Saint-Gall',NULL),(18,'Schaffhouse',NULL),(19,'Soleure',NULL),(20,'Schwyz',NULL),(21,'Thurgovie',NULL),(22,'Tessin',NULL),(23,'Uri',NULL),(24,'Vaud',NULL),(25,'Valais',NULL),(26,'Zoug',NULL),(27,'Zurich',NULL);
/*!40000 ALTER TABLE `regions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `beehives`
--

DROP TABLE IF EXISTS `beehives`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `beehives` (
  `beehive_id` int NOT NULL AUTO_INCREMENT,
  `beehive_name` varchar(50) NOT NULL,
  `beehive_number` varchar(50) DEFAULT NULL,
  `beehive_archive` date DEFAULT NULL,
  `fk_region_id` int DEFAULT NULL,
  PRIMARY KEY (`beehive_id`),
  KEY `idx_beehive_name` (`beehive_name`),
  KEY `fk_region_id` (`fk_region_id`),
  CONSTRAINT `beehives_ibfk_1` FOREIGN KEY (`fk_region_id`) REFERENCES `regions` (`region_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `beehives`
--

LOCK TABLES `beehives` WRITE;
/*!40000 ALTER TABLE `beehives` DISABLE KEYS */;
INSERT INTO `beehives` VALUES (1,'',NULL,'1901-01-01',1),(2,'La Côte',NULL,NULL,NULL),(3,'Le Laret',NULL,NULL,NULL),(4,'Petit Belmont',NULL,NULL,NULL);
/*!40000 ALTER TABLE `beehives` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clients`
--

DROP TABLE IF EXISTS `clients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clients` (
  `client_id` int NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int NOT NULL,
  PRIMARY KEY (`client_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `clients_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clients`
--

LOCK TABLES `clients` WRITE;
/*!40000 ALTER TABLE `clients` DISABLE KEYS */;
INSERT INTO `clients` VALUES (1,1),(5,2),(4,3),(3,4),(2,5),(7,6),(8,7),(11,8),(12,9),(6,13),(9,14),(10,15);
/*!40000 ALTER TABLE `clients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `entities`
--

DROP TABLE IF EXISTS `entities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `entities` (
  `entity_id` int NOT NULL AUTO_INCREMENT,
  `entity_name` varchar(50) NOT NULL,
  `entity_phone` varchar(25) DEFAULT NULL,
  `entity_email` varchar(255) DEFAULT NULL,
  `entity_city` varchar(50) DEFAULT NULL,
  `entity_address` varchar(255) DEFAULT NULL,
  `entity_archive` date DEFAULT NULL,
  PRIMARY KEY (`entity_id`),
  KEY `idx_entities_entity_name` (`entity_name`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `entities`
--

LOCK TABLES `entities` WRITE;
/*!40000 ALTER TABLE `entities` DISABLE KEYS */;
INSERT INTO `entities` VALUES (1,'Bluette','','','','',NULL),(2,'Roggen','','','','',NULL),(3,'Ruchat','','','','',NULL),(4,'Espace Abeilles','','','','',NULL),(5,'Carmelina','','','','',NULL),(6,'Chandines','','','','',NULL),(7,'Arbothévoz','','','','',NULL),(8,'Particuliers','','','','',NULL),(9,'Rithner','','','','',NULL),(10,'Bienen Meier','','','','',NULL),(11,'Apimat','','','','',NULL),(12,'Landi','','','','',NULL),(13,'Marché Ressudens 10.05.25','','','','',NULL),(14,'Marco location','','','','',NULL),(15,'Nicolas location','','','','',NULL),(16,'Corso Ivrea','','','','','2025-10-04'),(17,'JUMBO','','','','',NULL),(18,'Bastella','','','','',NULL),(19,'MIGROS','','','','',NULL),(20,'TEMPONNEMOI.COM','','','','',NULL),(21,'Frais divers','','','','',NULL),(22,'smartphoto','','','','',NULL),(23,'VBS','','','','',NULL),(24,'L\'PIXL','026 675 53 90','info@lpixl.ch','1580 Avenches','Rue Bibracte 4a',NULL),(25,'ROUTE D\'OR F','','','','',NULL),(26,'MANOR','','','','',NULL),(27,'Rue des Arts sàrl','','','1870 Monthey','Rue de Coppet 2',NULL),(28,'Ocres de France','0033 4 90 74 63 82','www.ocres-de-france.com','F 84400 Apt','Ch des Ocriers 200',NULL),(29,'Droguerie du Portail','026 660 25 18','','1530 Payerne','Grand Rue 64',NULL),(30,'SAR','','','','',NULL),(31,'COOP','','','','',NULL),(32,'Schilliger','','','','',NULL),(33,'FVA','','','','',NULL),(34,'Kurt Nobs','079 252 69 52','','1583 Villarepos','La Solitude 5',NULL),(36,'La cartoleria ENRICO','+39 0125 61 58 83','www.enricocart.it','10015 Ivrea','Corso Vercelli 334',NULL),(37,'Sanima','026 305 22 82','www.sanima.ch','1725 Posieux','Rte de Grangeneuve 21',NULL);
/*!40000 ALTER TABLE `entities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
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
  KEY `idx_products_product_name` (`product_name`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`fk_product_type_id`) REFERENCES `products_types` (`product_type_id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`fk_product_category_id`) REFERENCES `products_categories` (`product_category_id`),
  CONSTRAINT `products_ibfk_3` FOREIGN KEY (`fk_product_shape_id`) REFERENCES `products_shapes` (`product_shape_id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,'Miel 125g',125,NULL,1,1,1),(2,'Miel 250g',250,NULL,1,1,1),(3,'Miel 500g',500,NULL,1,1,1),(4,'Miel 1000g',1000,NULL,1,1,1),(5,'Bocaux, boîtes MeR',0,NULL,1,1,1),(6,'Etiquette',0,NULL,1,1,1),(7,'Moules bougies',0,NULL,1,2,1),(8,'Mèches bougies',0,NULL,1,2,1),(9,'Matière première',0,NULL,1,3,1),(10,'Tampon',0,NULL,1,3,1),(11,'Sacs',0,NULL,1,1,1),(12,'Carte/ visite',0,NULL,1,18,1),(13,'Emballage présentation',0,NULL,1,1,1),(14,'Peinture',0,NULL,2,10,1),(15,'Réparation',0,NULL,2,10,1),(16,'Nourissement',0,NULL,2,11,1),(17,'Traitement',0,NULL,2,11,1),(18,'Matériel apicole',0,NULL,2,11,1),(19,'Administratif',0,NULL,2,12,1),(20,'Bougie alvéoles grandes',0,NULL,1,2,1),(21,'Bougie alvéoles petites',0,NULL,1,2,1),(22,'Bougie 1001 Fleurs',0,NULL,1,2,1),(23,'Bougie longue',0,NULL,1,2,1),(24,'Bougie torsadée',0,NULL,1,2,1),(25,'Bougie ruche',0,NULL,1,2,1),(26,'Bougie hibou',0,NULL,1,2,1),(27,'Bougie florette',0,NULL,1,2,1),(28,'Bougie cire/béton',0,NULL,1,2,1),(29,'Cire bricolage',0,NULL,1,2,1),(30,'Bougie cire gaufrée',0,NULL,1,2,1),(31,'API\'SAVON laurier',70,NULL,1,3,1),(32,'Miel trio 125',125,NULL,1,1,1),(33,'Miel duo 125',125,NULL,1,1,1),(34,'Miel duo 250',250,NULL,1,1,1),(35,'Miel en rayons 100g',100,NULL,1,1,1),(36,'Décorations cire',0,NULL,1,2,1),(37,'Cire brut',100,NULL,1,2,1),(38,'Taxes/douane/livraison',0,NULL,1,14,1),(39,'Matériel décorations',0,NULL,1,4,1),(40,'Logo CT',0,NULL,1,18,1),(41,'Livres',0,NULL,2,16,1),(42,'Moules décos cire',0,NULL,1,2,1),(43,'Soins santé',0,NULL,2,15,1),(44,'Béton créatif',0,NULL,1,2,1),(45,'Contrôle miel',0,NULL,1,1,1),(46,'Reine Séléction',0,NULL,2,11,1),(47,'Miel en rayons 200g',200,NULL,1,1,1),(48,'Miel en rayons 500g',500,NULL,1,1,1),(49,'Miel en rayons 600g',600,NULL,1,1,1),(50,'Miel en rayons 400g',400,NULL,1,1,1);
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_categories`
--

DROP TABLE IF EXISTS `products_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products_categories` (
  `product_category_id` int NOT NULL AUTO_INCREMENT,
  `product_category_name` varchar(100) NOT NULL,
  `product_category_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_category_id`),
  KEY `idx_products_categories_product_category_name` (`product_category_name`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products_categories`
--

LOCK TABLES `products_categories` WRITE;
/*!40000 ALTER TABLE `products_categories` DISABLE KEYS */;
INSERT INTO `products_categories` VALUES (1,'Miel',NULL),(2,'Bougie/cire',NULL),(3,'Savon',NULL),(4,'Divers',NULL),(10,'Matériel entretien',NULL),(11,'Matériel colonies',NULL),(12,'Administratif',NULL),(13,'Cire','2025-08-19'),(14,'Frais divers/port',NULL),(15,'Bien-être Apicultrice',NULL),(16,'Formation/cours',NULL),(18,'Publicité',NULL);
/*!40000 ALTER TABLE `products_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_lots`
--

DROP TABLE IF EXISTS `products_lots`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products_lots` (
  `product_lot_id` int NOT NULL AUTO_INCREMENT,
  `product_lot_name` varchar(50) NOT NULL,
  `product_lot_year` int NOT NULL,
  `product_lot_archive` date DEFAULT NULL,
  `fk_beehive_id` int DEFAULT NULL,
  PRIMARY KEY (`product_lot_id`),
  KEY `fk_beehive_id` (`fk_beehive_id`),
  KEY `idx_products_lots_product_lot_name` (`product_lot_name`),
  KEY `idx_products_lots_product_lot_year` (`product_lot_year`),
  CONSTRAINT `products_lots_ibfk_1` FOREIGN KEY (`fk_beehive_id`) REFERENCES `beehives` (`beehive_id`)
) ENGINE=InnoDB AUTO_INCREMENT=19 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products_lots`
--

LOCK TABLES `products_lots` WRITE;
/*!40000 ALTER TABLE `products_lots` DISABLE KEYS */;
INSERT INTO `products_lots` VALUES (1,'',0,'1901-01-01',1),(2,'LL125',2025,NULL,3),(3,'LL225',2025,NULL,3),(4,'LC125',2025,NULL,2),(5,'LC225',2025,NULL,2),(6,'L124',2024,NULL,3),(7,'L224',2024,NULL,3),(8,'L324',2024,NULL,3),(9,'L241',2024,NULL,2),(10,'L242',2024,NULL,2),(11,'L243',2024,NULL,2),(12,'L123',2023,NULL,3),(13,'L223',2023,NULL,3),(14,'L231',2023,NULL,2),(15,'L232',2023,NULL,2),(16,'L1.25',2025,NULL,3),(17,'LCCR25',2025,NULL,2),(18,'LLCR25',2025,NULL,3);
/*!40000 ALTER TABLE `products_lots` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_shapes`
--

DROP TABLE IF EXISTS `products_shapes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products_shapes` (
  `product_shape_id` int NOT NULL AUTO_INCREMENT,
  `product_shape_name` varchar(100) NOT NULL,
  `product_shape_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_shape_id`),
  KEY `idx_products_shapes_product_shape_name` (`product_shape_name`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products_shapes`
--

LOCK TABLES `products_shapes` WRITE;
/*!40000 ALTER TABLE `products_shapes` DISABLE KEYS */;
INSERT INTO `products_shapes` VALUES (1,'','1901-01-01');
/*!40000 ALTER TABLE `products_shapes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_types`
--

DROP TABLE IF EXISTS `products_types`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products_types` (
  `product_type_id` int NOT NULL AUTO_INCREMENT,
  `product_type_name` varchar(100) NOT NULL,
  `product_type_archive` date DEFAULT NULL,
  PRIMARY KEY (`product_type_id`),
  KEY `idx_products_types_product_type_name` (`product_type_name`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products_types`
--

LOCK TABLES `products_types` WRITE;
/*!40000 ALTER TABLE `products_types` DISABLE KEYS */;
INSERT INTO `products_types` VALUES (1,'Production',NULL),(2,'Exploitation',NULL);
/*!40000 ALTER TABLE `products_types` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receipts`
--

DROP TABLE IF EXISTS `receipts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receipts` (
  `receipt_id` int NOT NULL AUTO_INCREMENT,
  `receipt_number` varchar(50) NOT NULL,
  `receipt_total_price` decimal(9,2) NOT NULL,
  `receipt_date_created` date NOT NULL,
  PRIMARY KEY (`receipt_id`),
  KEY `idx_receipts_receipt_number` (`receipt_number`),
  KEY `idx_receipts_receipt_total_price` (`receipt_total_price`),
  KEY `idx_receipts_receipt_date_created` (`receipt_date_created`)
) ENGINE=InnoDB AUTO_INCREMENT=93 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receipts`
--

LOCK TABLES `receipts` WRITE;
/*!40000 ALTER TABLE `receipts` DISABLE KEYS */;
INSERT INTO `receipts` VALUES (1,'74-25',101.00,'2025-06-11'),(2,'73-25',142.50,'2025-05-29'),(3,'EA0125',6.80,'2025-05-12'),(4,'71-25',87.00,'2025-05-28'),(5,'72-25',322.50,'2025-05-28'),(9,'Mar0225',698.16,'2025-05-10'),(10,'EA0525',105.45,'2025-09-12'),(11,'75-25',285.00,'2025-07-29'),(12,'76-25',116.00,'2025-07-31'),(13,'AT125',0.00,'2025-06-04'),(14,'AT225',0.00,'2025-08-11'),(15,'ML225',0.00,'2025-07-30'),(16,'ML125',0.00,'2025-05-29'),(17,'NL125',0.00,'2025-06-11'),(18,'NL225',0.00,'2025-07-31'),(19,'EA0325',143.70,'2025-08-08'),(20,'EA0225',11.90,'2025-06-16'),(21,'P30525',78.00,'2025-05-30'),(22,'P40525',330.00,'2025-05-31'),(23,'P10625',0.00,'2025-06-01'),(24,'P30625',22.00,'2025-06-20'),(25,'P10725',85.00,'2025-07-01'),(26,'P20725',44.00,'2025-07-21'),(27,'P20825',22.00,'2025-08-12'),(28,'P10525',50.00,'2025-05-23'),(29,'P20525',0.00,'2025-05-24'),(30,'P20625',55.00,'2025-06-04'),(31,'P40625',24.00,'2025-06-27'),(32,'P30725',13.00,'2025-07-21'),(33,'P40725',0.00,'2025-07-27'),(34,'P10825',98.00,'2025-08-06'),(35,'P30825',44.00,'2025-08-12'),(36,'001',19.80,'2025-03-27'),(37,'244',51.45,'2025-01-20'),(38,'245',65.60,'2025-01-24'),(39,'246',19.45,'2025-01-30'),(40,'247',21.80,'2025-02-01'),(41,'248',31.95,'2025-02-04'),(42,'249',19.95,'2025-02-13'),(43,'249B',4.60,'2025-02-13'),(44,'250',9.90,'2025-02-13'),(45,'251',23.60,'2025-02-14'),(46,'252',78.80,'2025-02-27'),(47,'253',302.70,'2025-02-28'),(48,'254',14.95,'2025-03-06'),(49,'255',124.50,'2025-03-11'),(50,'256',23.45,'2025-03-17'),(51,'257',61.85,'2025-03-27'),(52,'258',212.60,'2025-03-27'),(53,'259',16.00,'2025-03-27'),(54,'258B',11.90,'2025-03-27'),(55,'260',7.45,'2025-03-27'),(56,'261',23.45,'2025-03-29'),(57,'262',102.20,'2025-03-29'),(58,'263',49.60,'2025-04-10'),(59,'264',14.95,'2025-04-30'),(60,'265',30.30,'2025-05-02'),(61,'266',2.95,'2025-05-01'),(62,'267',30.00,'2025-05-10'),(63,'268',168.40,'2025-05-24'),(64,'269',13.50,'2025-05-26'),(65,'270',32.40,'2025-06-02'),(66,'271',179.60,'2025-06-09'),(67,'272',42.00,'2025-06-18'),(68,'273',75.00,'2025-07-11'),(69,'274',12.00,'2025-07-28'),(70,'EA0425',35.70,'2025-08-29'),(71,'P50825',0.00,'2025-08-30'),(72,'P10925',48.00,'2025-09-08'),(73,'P20925',0.00,'2025-09-27'),(74,'275',32.40,'2025-08-08'),(75,'276',21.00,'2025-08-26'),(76,'277',21.00,'2025-09-08'),(77,'278',64.70,'2025-09-20'),(78,'279',24.90,'2025-09-27'),(79,'77-25',136.00,'2025-10-08'),(80,'P11025',0.00,'2025-10-23'),(81,'280',14.95,'2025-10-20'),(82,'281',16.90,'2025-10-29'),(83,'282',86.00,'2025-10-30'),(84,'P40825',11.00,'2025-08-20'),(85,'EA0625',106.50,'2025-11-10'),(86,'78-25',90.00,'2025-11-13'),(87,'79-25',126.00,'2025-11-15'),(88,'P11125',0.00,'2025-11-16'),(89,'P21125',40.00,'2025-11-29'),(90,'P11225',22.00,'2025-12-18'),(91,'283',87.00,'2026-12-29'),(92,'EA0725',5.10,'2025-12-30');
/*!40000 ALTER TABLE `receipts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receipts_clients`
--

DROP TABLE IF EXISTS `receipts_clients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receipts_clients` (
  `fk_receipt_id` int NOT NULL,
  `fk_client_id` int NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_client_id` (`fk_client_id`),
  CONSTRAINT `receipts_clients_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_clients_ibfk_2` FOREIGN KEY (`fk_client_id`) REFERENCES `clients` (`client_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receipts_clients`
--

LOCK TABLES `receipts_clients` WRITE;
/*!40000 ALTER TABLE `receipts_clients` DISABLE KEYS */;
INSERT INTO `receipts_clients` VALUES (3,3),(28,11),(29,11),(21,11),(22,11),(23,11),(30,11),(24,11),(31,11),(25,11),(26,11),(32,11),(34,11),(27,11),(35,11),(16,9),(15,9),(17,10),(18,10),(13,8),(14,8),(4,4),(5,5),(2,2),(1,1),(11,7),(12,1),(79,4),(20,3),(9,6),(80,11),(84,11),(71,11),(86,5),(87,7),(72,11),(10,3),(73,11),(85,3),(19,3),(70,3),(88,11),(89,11),(33,11),(90,11),(92,3),(36,12);
/*!40000 ALTER TABLE `receipts_clients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receipts_products`
--

DROP TABLE IF EXISTS `receipts_products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receipts_products` (
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
  KEY `idx_receipts_products_receipt_product_quantity` (`receipt_product_quantity`),
  KEY `idx_receipts_products_receipt_product_unity_price` (`receipt_product_unity_price`),
  CONSTRAINT `receipts_products_ibfk_1` FOREIGN KEY (`fk_product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `receipts_products_ibfk_2` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_products_ibfk_3` FOREIGN KEY (`fk_product_lot_id`) REFERENCES `products_lots` (`product_lot_id`)
) ENGINE=InnoDB AUTO_INCREMENT=1040 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receipts_products`
--

LOCK TABLES `receipts_products` WRITE;
/*!40000 ALTER TABLE `receipts_products` DISABLE KEYS */;
INSERT INTO `receipts_products` VALUES (510,1,3.40,25,3,17),(511,1,3.40,26,3,17),(518,2,25.00,4,28,4),(519,1,0.00,2,29,4),(520,6,13.00,3,21,2),(521,12,22.00,4,22,2),(522,3,22.00,4,22,4),(523,1,0.00,3,23,2),(524,1,0.00,3,23,5),(525,5,11.00,3,30,4),(526,1,11.00,3,24,2),(527,1,11.00,3,24,4),(528,1,24.00,4,31,4),(529,5,11.00,3,25,2),(530,5,6.00,2,25,2),(531,3,11.00,3,26,2),(532,1,11.00,3,26,4),(533,1,13.00,3,32,5),(536,3,14.00,3,34,3),(537,4,14.00,3,34,5),(538,1,11.00,3,27,2),(539,1,11.00,3,27,3),(540,2,11.00,3,35,3),(541,2,11.00,3,35,5),(543,2,0.00,3,16,4),(544,3,0.00,3,15,5),(545,3,0.00,3,17,2),(546,4,0.00,3,18,3),(547,10,0.00,3,13,4),(548,5,0.00,3,14,5),(586,6,9.50,3,4,2),(587,6,5.00,2,4,2),(588,10,18.00,4,5,2),(589,15,9.50,3,5,2),(590,15,9.50,3,2,4),(591,8,9.50,3,1,2),(592,5,5.00,2,1,2),(593,5,9.50,3,11,4),(594,25,9.50,3,11,5),(595,8,9.50,3,12,3),(596,3,5.00,2,12,2),(597,5,5.00,2,12,3),(681,1,51.45,13,37,1),(682,1,44.90,10,38,1),(683,1,13.70,40,38,1),(684,1,7.00,38,38,1),(685,1,19.45,38,39,1),(686,1,4.30,8,40,1),(687,1,17.50,39,40,1),(690,1,4.60,9,43,1),(691,1,6.95,39,44,1),(692,1,2.95,39,44,1),(693,8,2.95,39,45,1),(694,1,13.35,42,46,1),(695,1,7.10,42,46,1),(696,2,16.30,42,46,1),(697,1,10.35,42,46,1),(698,1,8.45,42,46,1),(699,1,6.95,38,46,1),(700,1,302.70,40,47,1),(702,50,1.34,13,49,1),(703,1,27.50,41,49,1),(704,1,30.00,38,49,1),(705,1,23.45,38,50,1),(706,1,12.60,42,51,1),(707,1,25.90,42,51,1),(708,2,4.65,39,51,1),(709,2,3.55,39,51,1),(710,1,6.95,38,51,1),(711,48,1.00,5,52,1),(712,28,0.95,5,52,1),(713,40,0.90,5,52,1),(714,1,38.00,41,52,1),(715,1,15.00,43,52,1),(716,1,10.00,43,52,1),(717,1,18.00,43,52,1),(718,1,21.00,43,52,1),(719,1,16.00,39,53,1),(734,2,5.95,39,54,1),(735,1,7.45,39,55,1),(736,2,3.25,39,56,1),(737,1,4.50,39,56,1),(738,1,3.50,39,56,1),(739,1,8.95,39,56,1),(740,1,12.90,41,57,1),(741,1,1.36,14,57,1),(742,2,1.34,14,57,1),(743,1,7.75,14,57,1),(744,1,7.40,14,57,1),(745,1,5.41,14,57,1),(746,1,3.68,14,57,1),(747,1,61.02,38,57,1),(748,1,9.50,9,58,1),(749,1,26.20,9,58,1),(750,1,13.90,9,58,1),(751,1,14.95,44,59,1),(752,1,2.95,37,61,1),(753,1,30.30,38,60,1),(754,1,30.00,38,62,1),(760,2,3.00,45,64,1),(761,75,0.10,45,64,1),(762,30,1.08,16,65,1),(763,1,89.90,18,66,1),(764,3,29.90,18,66,1),(765,2,14.00,17,67,1),(766,1,14.00,17,67,1),(767,1,75.00,46,68,1),(768,2,3.00,45,69,1),(769,60,0.10,45,69,1),(770,30,1.08,16,74,1),(772,7,3.00,19,75,1),(773,20,1.05,16,76,1),(774,1,64.70,6,77,1),(775,1,24.90,11,78,1),(776,1,14.95,44,48,1),(777,8,9.50,3,79,3),(778,12,5.00,2,79,3),(779,40,0.86,5,63,1),(780,1,75.00,18,63,1),(781,1,15.00,18,63,1),(782,10,1.50,5,63,1),(783,10,2.90,5,63,1),(784,1,19.95,39,42,1),(785,1,31.95,12,41,1),(852,1,3.40,25,20,17),(853,1,3.40,26,20,17),(854,1,5.10,28,20,17),(891,1,6.00,21,9,17),(892,2,4.00,24,9,17),(893,5,4.00,25,9,17),(894,5,4.00,26,9,17),(895,1,3.00,27,9,17),(896,1,6.00,28,9,17),(897,1,6.00,30,9,17),(898,2,7.50,29,9,17),(899,6,13.00,3,9,6),(900,6,13.00,3,9,7),(901,6,13.00,3,9,8),(902,6,13.00,3,9,11),(903,7,7.00,2,9,8),(904,5,7.00,2,9,11),(905,1,9.00,34,9,8),(906,1,9.00,34,9,11),(907,3,4.50,1,9,6),(908,3,4.50,1,9,7),(909,2,4.50,1,9,8),(910,3,4.50,1,9,9),(911,4,4.50,1,9,10),(912,2,4.50,1,9,11),(913,1,4.50,1,9,12),(914,1,4.50,1,9,13),(915,1,4.50,1,9,14),(916,2,4.50,1,9,15),(917,1,5.66,32,9,12),(918,1,5.66,32,9,6),(919,1,5.66,32,9,7),(920,1,5.66,32,9,9),(921,1,5.66,1,9,11),(922,1,5.66,1,9,15),(923,8,6.50,31,9,16),(924,3,2.00,36,9,17),(925,1,4.25,35,9,8),(926,1,4.95,35,9,8),(928,1,0.00,2,80,5),(929,1,14.95,44,81,1),(930,1,14.95,44,82,1),(931,1,1.95,8,82,1),(932,1,86.00,19,83,1),(939,1,11.00,3,84,5),(940,1,0.00,3,71,4),(957,5,18.00,4,86,3),(959,10,5.00,2,87,5),(960,8,9.50,3,87,5),(961,1,24.00,4,72,2),(962,1,24.00,4,72,5),(983,3,5.10,32,10,2),(984,3,5.10,32,10,3),(985,3,5.10,32,10,5),(986,1,5.10,33,10,3),(987,1,5.10,33,10,5),(988,3,0.85,36,10,18),(989,1,7.20,47,10,2),(990,1,8.25,47,10,2),(991,1,8.65,47,10,2),(992,1,22.70,49,10,2),(993,1,0.00,32,73,2),(994,1,0.00,32,73,3),(995,1,0.00,32,73,5),(996,1,0.00,36,73,18),(997,3,5.10,21,85,18),(998,2,3.40,23,85,18),(999,1,2.55,27,85,18),(1000,1,17.90,50,85,2),(1001,1,20.75,48,85,2),(1002,1,22.80,49,85,2),(1003,2,5.10,33,85,3),(1004,2,5.10,33,85,5),(1005,6,7.65,2,19,2),(1006,4,7.65,2,19,4),(1007,1,5.10,33,19,2),(1008,1,5.10,33,19,4),(1009,1,7.65,47,19,2),(1010,1,7.70,47,19,2),(1011,1,15.30,20,19,18),(1012,1,3.40,26,19,18),(1013,3,2.55,27,19,18),(1014,3,5.10,28,19,18),(1015,1,5.10,21,70,18),(1016,3,5.10,33,70,2),(1017,3,5.10,33,70,4),(1019,1,0.00,26,88,18),(1020,4,10.00,3,89,5),(1021,1,0.00,4,33,3),(1022,1,0.00,4,33,5),(1023,1,22.00,4,90,5),(1030,1,22.00,7,91,1),(1031,1,23.00,7,91,1),(1032,1,29.00,7,91,1),(1033,1,13.00,38,91,1),(1034,1,5.10,21,92,18),(1039,22,0.90,37,36,17);
/*!40000 ALTER TABLE `receipts_products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `receipts_suppliers`
--

DROP TABLE IF EXISTS `receipts_suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `receipts_suppliers` (
  `fk_receipt_id` int NOT NULL,
  `fk_supplier_id` int NOT NULL,
  KEY `fk_receipt_id` (`fk_receipt_id`),
  KEY `fk_supplier_id` (`fk_supplier_id`),
  CONSTRAINT `receipts_suppliers_ibfk_1` FOREIGN KEY (`fk_receipt_id`) REFERENCES `receipts` (`receipt_id`),
  CONSTRAINT `receipts_suppliers_ibfk_2` FOREIGN KEY (`fk_supplier_id`) REFERENCES `suppliers` (`supplier_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `receipts_suppliers`
--

LOCK TABLES `receipts_suppliers` WRITE;
/*!40000 ALTER TABLE `receipts_suppliers` DISABLE KEYS */;
INSERT INTO `receipts_suppliers` VALUES (37,26),(38,2),(39,3),(40,4),(43,7),(44,8),(45,8),(46,9),(47,10),(49,12),(50,3),(51,9),(52,13),(53,14),(54,15),(55,6),(56,6),(57,16),(58,17),(59,6),(61,18),(60,3),(62,19),(64,21),(65,22),(66,23),(67,24),(68,25),(69,21),(74,22),(75,27),(76,22),(77,5),(78,26),(48,6),(63,20),(42,6),(41,5),(81,6),(82,6),(83,21),(91,20);
/*!40000 ALTER TABLE `receipts_suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `fk_entity_id` int NOT NULL,
  PRIMARY KEY (`supplier_id`),
  KEY `fk_entity_id` (`fk_entity_id`),
  CONSTRAINT `suppliers_ibfk_1` FOREIGN KEY (`fk_entity_id`) REFERENCES `entities` (`entity_id`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (11,1),(7,2),(13,9),(20,11),(18,12),(19,13),(1,16),(6,17),(4,18),(8,19),(2,20),(3,21),(5,22),(9,23),(10,24),(12,25),(15,26),(14,27),(16,28),(17,29),(21,30),(22,31),(23,32),(24,33),(25,34),(26,36),(27,37);
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `dose_units`
--

DROP TABLE IF EXISTS `dose_units`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `dose_units` (
  `dose_unit_id` int NOT NULL AUTO_INCREMENT,
  `dose_unit_name` varchar(100) NOT NULL,
  `dose_unit_archive` date DEFAULT NULL,
  PRIMARY KEY (`dose_unit_id`),
  KEY `idx_dose_unit_name` (`dose_unit_name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `dose_units`
--

LOCK TABLES `dose_units` WRITE;
/*!40000 ALTER TABLE `dose_units` DISABLE KEYS */;
/*!40000 ALTER TABLE `dose_units` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `treatments`
--

DROP TABLE IF EXISTS `treatments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `treatments` (
  `treatment_id` int NOT NULL AUTO_INCREMENT,
  `treatment_date` date NOT NULL,
  `treatment_hive_count` int NOT NULL,
  `treatment_dose_per_hive` decimal(9,2) NOT NULL,
  `fk_beehive_id` int NOT NULL,
  `fk_product_id` int NOT NULL,
  `fk_dose_unit_id` int DEFAULT NULL,
  PRIMARY KEY (`treatment_id`),
  KEY `idx_treatment_date` (`treatment_date`),
  KEY `fk_beehive_id` (`fk_beehive_id`),
  KEY `fk_product_id` (`fk_product_id`),
  KEY `fk_dose_unit_id` (`fk_dose_unit_id`),
  CONSTRAINT `treatments_ibfk_1` FOREIGN KEY (`fk_beehive_id`) REFERENCES `beehives` (`beehive_id`),
  CONSTRAINT `treatments_ibfk_2` FOREIGN KEY (`fk_product_id`) REFERENCES `products` (`product_id`),
  CONSTRAINT `treatments_ibfk_3` FOREIGN KEY (`fk_dose_unit_id`) REFERENCES `dose_units` (`dose_unit_id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `treatments`
--

LOCK TABLES `treatments` WRITE;
/*!40000 ALTER TABLE `treatments` DISABLE KEYS */;
/*!40000 ALTER TABLE `treatments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `user_name` varchar(50) NOT NULL,
  `user_phone` varchar(25) DEFAULT NULL,
  `user_email` varchar(255) DEFAULT NULL,
  `user_city` varchar(50) DEFAULT NULL,
  `user_address` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`user_id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'Corinne Thumelin','026 667 11 78','','1773 Russy','Rte de l\'Ecole 12');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping events for database 'mams_db'
--

--
-- Dumping routines for database 'mams_db'
--
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-09 16:36:42
