-- MySQL dump 10.13  Distrib 8.0.43, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: proyectopoo
-- ------------------------------------------------------
-- Server version	8.0.43

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
-- Table structure for table `categorias`
--

DROP TABLE IF EXISTS `categorias`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categorias` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(60) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Descripcion` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categorias`
--

LOCK TABLES `categorias` WRITE;
/*!40000 ALTER TABLE `categorias` DISABLE KEYS */;
INSERT INTO `categorias` VALUES (1,'General','Productos sin categorias'),(2,'Bebidas','Bebidas y refrescos'),(3,'Dulces','Paletas, chocolates y chicles'),(4,'Postres','Galletas y pan dulce'),(5,'Frutas y verduras','Frutas y verduras'),(6,'Sabritas','Sabritas y papitas'),(7,'Aguas frescas','Aguas de sabores');
/*!40000 ALTER TABLE `categorias` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `clientes`
--

DROP TABLE IF EXISTS `clientes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `clientes` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Telefono` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Email` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `clientes`
--

LOCK TABLES `clientes` WRITE;
/*!40000 ALTER TABLE `clientes` DISABLE KEYS */;
INSERT INTO `clientes` VALUES (1,'Público en general',NULL,NULL),(2,'David Ponce','6242301968','siqueirosdavid200@gmail.com');
/*!40000 ALTER TABLE `clientes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `configuracion`
--

DROP TABLE IF EXISTS `configuracion`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `configuracion` (
  `Clave` varchar(50) NOT NULL,
  `ValorDecimal` decimal(10,4) DEFAULT NULL,
  `ValorTexto` varchar(200) DEFAULT NULL,
  PRIMARY KEY (`Clave`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `configuracion`
--

LOCK TABLES `configuracion` WRITE;
/*!40000 ALTER TABLE `configuracion` DISABLE KEYS */;
INSERT INTO `configuracion` VALUES ('ComisionTarjeta',0.0350,NULL);
/*!40000 ALTER TABLE `configuracion` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `detalles`
--

DROP TABLE IF EXISTS `detalles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `detalles` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `VentaId` int NOT NULL,
  `ProductoId` int NOT NULL,
  `NombreProducto` varchar(120) NOT NULL,
  `PrecioUnitario` decimal(10,2) NOT NULL,
  `Cantidad` int NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Det_Venta` (`VentaId`),
  KEY `FK_Det_Prod` (`ProductoId`),
  CONSTRAINT `FK_Det_Prod` FOREIGN KEY (`ProductoId`) REFERENCES `productos` (`Id`),
  CONSTRAINT `FK_Det_Venta` FOREIGN KEY (`VentaId`) REFERENCES `ventas` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=59 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `detalles`
--

LOCK TABLES `detalles` WRITE;
/*!40000 ALTER TABLE `detalles` DISABLE KEYS */;
INSERT INTO `detalles` VALUES (1,1,2,'Gomitas',10.00,2),(2,1,1,'Coca 600ml',15.00,1),(3,2,1,'Coca 600ml',15.00,1),(4,2,2,'Gomitas',10.00,1),(5,3,1,'Coca 600ml',15.00,1),(6,4,1,'Coca 600ml',25.00,1),(7,5,1,'Coca 600ml',25.00,1),(8,5,1,'Coca 600ml',25.00,5),(9,5,1,'Coca 600ml',25.00,11),(10,5,1,'Coca 600ml',25.00,14),(11,5,1,'Coca 600ml',25.00,16),(12,6,1,'Coca 600ml',25.00,1),(13,7,1,'Coca 600ml',25.00,1),(14,7,8,'Donitas bimbo',25.00,1),(15,7,2,'Gomitas',10.00,1),(16,8,13,'Pinguinos',25.00,1),(17,9,10,'Cilantro',5.00,1),(18,10,10,'Cilantro',5.00,1),(19,11,10,'Cilantro',5.00,1),(20,12,10,'Cilantro',5.00,1),(21,13,14,'Tortillas',30.00,1),(22,14,14,'Tortillas',30.00,1),(23,15,14,'Tortillas',30.00,1),(24,16,14,'Tortillas',30.00,1),(25,17,1,'Coca 600ml',25.00,1),(26,18,1,'Coca 600ml',25.00,1),(27,19,14,'Tortillas',30.00,1),(28,21,12,'Napolitanos',25.00,1),(29,22,11,'Pepsi 600ml',20.00,1),(30,23,9,'Limon',3.00,1),(31,24,14,'Tortillas',30.00,1),(32,25,2,'Gomitas',10.00,1),(33,26,9,'Limon',3.00,1),(34,27,10,'Cilantro',5.00,1),(35,28,11,'Pepsi 600ml',20.00,1),(36,29,11,'Pepsi 600ml',20.00,3),(37,30,9,'Limon',3.00,1),(38,31,9,'Limon',3.00,1),(39,32,11,'Pepsi 600ml',20.00,1),(40,33,11,'Pepsi 600ml',20.00,1),(41,34,10,'Cilantro',5.00,1),(42,35,14,'Tortillas',30.00,1),(43,36,1,'Coca 600ml',25.00,1),(44,37,10,'Cilantro',5.00,1),(45,38,12,'Napolitanos',25.00,1),(46,39,13,'Pinguinos',25.00,1),(47,40,15,'Coca 600ml Zero',30.00,1),(48,41,1,'Coca 600ml',25.00,1),(49,41,16,'Chips moradas',27.00,1),(50,41,18,'Pan',9.00,1),(51,42,18,'Pan',9.00,1),(52,43,17,'Cheetos naranja',25.00,1),(53,44,20,'Agua',25.00,1),(54,45,20,'Agua',25.00,1),(55,46,20,'Agua',25.00,1),(56,47,22,'Agua horchata',25.00,1),(57,48,22,'Agua horchata',25.00,1),(58,49,20,'Agua',25.00,1);
/*!40000 ALTER TABLE `detalles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `empleados`
--

DROP TABLE IF EXISTS `empleados`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `empleados` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(120) NOT NULL,
  `Rol` varchar(60) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `empleados`
--

LOCK TABLES `empleados` WRITE;
/*!40000 ALTER TABLE `empleados` DISABLE KEYS */;
INSERT INTO `empleados` VALUES (1,'David','Cajero'),(2,'Hola','Cajero'),(3,'Hiram','Cajero');
/*!40000 ALTER TABLE `empleados` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `movimientosinventario`
--

DROP TABLE IF EXISTS `movimientosinventario`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `movimientosinventario` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ProductoId` int NOT NULL,
  `Fecha` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `Tipo` varchar(10) NOT NULL,
  `Cantidad` int NOT NULL,
  `Motivo` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `ProductoId` (`ProductoId`),
  CONSTRAINT `movimientosinventario_ibfk_1` FOREIGN KEY (`ProductoId`) REFERENCES `productos` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `movimientosinventario`
--

LOCK TABLES `movimientosinventario` WRITE;
/*!40000 ALTER TABLE `movimientosinventario` DISABLE KEYS */;
INSERT INTO `movimientosinventario` VALUES (1,11,'2025-10-15 14:58:49','entrada',2,NULL),(2,1,'2025-10-15 14:58:56','entrada',2,NULL),(3,8,'2025-10-15 14:59:19','salida',1,'Se perdio'),(4,15,'2025-10-15 15:03:12','salida',20,'Porque si'),(5,15,'2025-10-15 17:59:58','entrada',5,NULL),(6,16,'2025-10-15 18:07:16','salida',19,'nuevo producto'),(7,17,'2025-10-15 18:16:01','entrada',21,NULL),(8,16,'2025-10-15 18:16:07','entrada',15,NULL),(9,24,'2025-10-16 14:41:47','entrada',5,NULL),(10,24,'2025-10-16 14:45:38','entrada',1,NULL),(11,24,'2025-10-16 14:46:31','salida',1,'si');
/*!40000 ALTER TABLE `movimientosinventario` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `productos`
--

DROP TABLE IF EXISTS `productos`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `productos` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Precio` decimal(65,30) NOT NULL,
  `Stock` int NOT NULL,
  `CategoriaId` int NOT NULL,
  `Activo` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`Id`),
  KEY `IX_Productos_CategoriaId` (`CategoriaId`),
  CONSTRAINT `FK_Productos_Categorias_CategoriaId` FOREIGN KEY (`CategoriaId`) REFERENCES `categorias` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `productos`
--

LOCK TABLES `productos` WRITE;
/*!40000 ALTER TABLE `productos` DISABLE KEYS */;
INSERT INTO `productos` VALUES (1,'Coca 600ml',25.000000000000000000000000000000,15,2,_binary ''),(2,'Gomitas',10.000000000000000000000000000000,34,3,_binary ''),(8,'Donitas bimbo',25.000000000000000000000000000000,13,4,_binary ''),(9,'Limon',3.000000000000000000000000000000,43,5,_binary ''),(10,'Cilantro',10.000000000000000000000000000000,17,5,_binary ''),(11,'Pepsi 600ml',20.000000000000000000000000000000,35,2,_binary ''),(12,'Napolitanos',25.000000000000000000000000000000,15,4,_binary ''),(13,'Pinguinos',25.000000000000000000000000000000,15,4,_binary ''),(14,'Tortillas',30.000000000000000000000000000000,35,1,_binary ''),(15,'Coca 600ml Zero',30.000000000000000000000000000000,20,2,_binary ''),(16,'Chips moradas',27.000000000000000000000000000000,16,6,_binary ''),(17,'Cheetos',25.000000000000000000000000000000,22,6,_binary ''),(18,'Pan',9.000000000000000000000000000000,1,4,_binary '\0'),(19,'Pan',9.000000000000000000000000000000,1,4,_binary '\0'),(20,'Agua',25.000000000000000000000000000000,1,2,_binary '\0'),(21,'Cheetos',25.000000000000000000000000000000,1,6,_binary '\0'),(22,'Agua horchata',25.000000000000000000000000000000,22,2,_binary ''),(23,'Agua jamaica',25.000000000000000000000000000000,12,2,_binary ''),(24,'Agua tamarindo',25.000000000000000000000000000000,6,2,_binary '');
/*!40000 ALTER TABLE `productos` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `proveedores`
--

DROP TABLE IF EXISTS `proveedores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `proveedores` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(120) NOT NULL,
  `Telefono` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `proveedores`
--

LOCK TABLES `proveedores` WRITE;
/*!40000 ALTER TABLE `proveedores` DISABLE KEYS */;
INSERT INTO `proveedores` VALUES (1,'Bimbo','6241234567'),(3,'Lizarraga','6241234568');
/*!40000 ALTER TABLE `proveedores` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `usuarios`
--

DROP TABLE IF EXISTS `usuarios`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `usuarios` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Username` varchar(60) NOT NULL,
  `PasswordHash` char(64) NOT NULL,
  `Nombre` varchar(120) DEFAULT NULL,
  `IsAdmin` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Username` (`Username`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `usuarios`
--

LOCK TABLES `usuarios` WRITE;
/*!40000 ALTER TABLE `usuarios` DISABLE KEYS */;
INSERT INTO `usuarios` VALUES (1,'admin','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','Administrador',_binary ''),(3,'cajero','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','david',_binary '\0'),(4,'hola','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','hiram',_binary '\0'),(5,'hola2','a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3','peterch',_binary '');
/*!40000 ALTER TABLE `usuarios` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ventas`
--

DROP TABLE IF EXISTS `ventas`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ventas` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `ClienteId` int NOT NULL,
  `Fecha` datetime NOT NULL,
  `Total` decimal(10,2) DEFAULT NULL,
  `TipoPago` varchar(20) DEFAULT NULL,
  `MetodoPago` varchar(10) DEFAULT 'Efectivo',
  `EmpleadoId` int DEFAULT NULL,
  PRIMARY KEY (`Id`),
  KEY `FK_Venta_Cliente` (`ClienteId`),
  CONSTRAINT `FK_Venta_Cliente` FOREIGN KEY (`ClienteId`) REFERENCES `clientes` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=50 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ventas`
--

LOCK TABLES `ventas` WRITE;
/*!40000 ALTER TABLE `ventas` DISABLE KEYS */;
INSERT INTO `ventas` VALUES (1,1,'2025-10-12 16:47:27',35.00,NULL,'Efectivo',NULL),(2,1,'2025-10-12 16:47:58',25.00,NULL,'Efectivo',NULL),(3,1,'2025-10-12 17:16:17',15.00,NULL,'Efectivo',NULL),(4,1,'2025-10-12 18:21:34',25.00,NULL,'Efectivo',NULL),(5,1,'2025-10-12 19:27:13',1175.00,NULL,'Efectivo',NULL),(6,1,'2025-10-12 21:39:12',25.00,NULL,'Efectivo',NULL),(7,1,'2025-10-13 07:25:25',60.00,NULL,'Efectivo',NULL),(8,1,'2025-10-14 07:49:52',25.00,NULL,'Efectivo',NULL),(9,1,'2025-10-14 15:13:12',5.00,NULL,'Efectivo',NULL),(10,1,'2025-10-14 15:19:06',5.00,NULL,'Efectivo',NULL),(11,2,'2025-10-14 15:19:12',5.00,NULL,'Efectivo',NULL),(12,2,'2025-10-14 15:26:04',5.00,NULL,'Efectivo',NULL),(13,2,'2025-10-14 16:05:47',30.00,NULL,'Efectivo',NULL),(14,2,'2025-10-14 16:07:12',30.00,NULL,'Efectivo',NULL),(15,2,'2025-10-14 16:20:59',31.05,NULL,'Efectivo',NULL),(16,2,'2025-10-14 16:22:50',31.05,NULL,'Efectivo',NULL),(17,2,'2025-10-14 16:33:45',25.88,NULL,'Efectivo',NULL),(18,2,'2025-10-14 17:44:11',25.88,NULL,'Efectivo',NULL),(19,2,'2025-10-14 17:49:39',31.05,NULL,'Efectivo',NULL),(20,1,'2025-10-14 18:06:28',NULL,'Tarjeta','Efectivo',NULL),(21,2,'2025-10-14 18:08:52',25.88,NULL,'Efectivo',NULL),(22,2,'2025-10-14 18:09:22',20.70,NULL,'Efectivo',NULL),(23,2,'2025-10-14 18:14:26',3.11,NULL,'Efectivo',NULL),(24,2,'2025-10-14 18:27:26',31.05,NULL,'Tarjeta',NULL),(25,1,'2025-10-14 18:27:39',10.35,NULL,'Tarjeta',NULL),(26,2,'2025-10-14 18:37:47',3.11,NULL,'Tarjeta',NULL),(27,2,'2025-10-14 18:37:51',5.18,NULL,'Tarjeta',NULL),(28,2,'2025-10-14 18:37:58',20.70,NULL,'Tarjeta',NULL),(29,2,'2025-10-14 18:38:12',60.00,NULL,'Efectivo',NULL),(30,2,'2025-10-14 18:39:03',3.00,NULL,'Efectivo',NULL),(31,2,'2025-10-14 18:44:20',3.00,NULL,'Efectivo',NULL),(32,2,'2025-10-14 18:44:25',20.70,NULL,'Tarjeta',NULL),(33,1,'2025-10-14 18:48:24',20.00,NULL,'Efectivo',NULL),(34,1,'2025-10-14 18:48:30',5.00,NULL,'Efectivo',NULL),(35,2,'2025-10-14 18:54:22',31.05,NULL,'Tarjeta',1),(36,2,'2025-10-14 18:57:35',25.00,NULL,'Efectivo',1),(37,2,'2025-10-14 19:11:19',5.00,NULL,'Efectivo',2),(38,2,'2025-10-15 08:30:26',25.88,NULL,'Tarjeta',2),(39,1,'2025-10-15 13:27:35',25.88,NULL,'Tarjeta',1),(40,1,'2025-10-15 15:03:29',31.05,NULL,'Tarjeta',2),(41,1,'2025-10-16 07:20:32',61.00,NULL,'Efectivo',2),(42,2,'2025-10-16 07:25:36',9.00,NULL,'Efectivo',1),(43,2,'2025-10-16 08:21:13',25.00,NULL,'Efectivo',1),(44,2,'2025-10-16 08:23:09',25.00,NULL,'Efectivo',1),(45,2,'2025-10-16 14:13:17',25.00,NULL,'Efectivo',3),(46,2,'2025-10-16 14:14:50',25.88,NULL,'Tarjeta',1),(47,1,'2025-10-16 14:59:09',25.88,NULL,'Tarjeta',3),(48,2,'2025-10-16 15:05:27',25.00,NULL,'Efectivo',1),(49,2,'2025-10-16 15:10:56',25.00,NULL,'Efectivo',1);
/*!40000 ALTER TABLE `ventas` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-10-16 15:25:34
