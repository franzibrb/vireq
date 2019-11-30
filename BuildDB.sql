-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server Version:               10.4.10-MariaDB - mariadb.org binary distribution
-- Server Betriebssystem:        Win64
-- HeidiSQL Version:             10.2.0.5599
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;

-- Exportiere Struktur von Tabelle vireq.artikel
CREATE TABLE IF NOT EXISTS `artikel` (
  `ArtikelId` int(11) NOT NULL AUTO_INCREMENT,
  `PaletteId` int(11) NOT NULL DEFAULT 0,
  `Artikelnummer` varchar(50) COLLATE latin1_bin NOT NULL DEFAULT '0',
  `Artikelname` varchar(50) COLLATE latin1_bin NOT NULL DEFAULT '0',
  PRIMARY KEY (`ArtikelId`),
  KEY `FK_artikel_paletten` (`PaletteId`),
  CONSTRAINT `FK_artikel_paletten` FOREIGN KEY (`PaletteId`) REFERENCES `paletten` (`PaletteId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=latin1 COLLATE=latin1_bin;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle vireq.lieferanten
CREATE TABLE IF NOT EXISTS `lieferanten` (
  `LieferantId` int(11) NOT NULL AUTO_INCREMENT,
  `UserId` int(11) NOT NULL,
  `Lieferantennummer` int(11) NOT NULL,
  `Lieferantenname` varchar(50) COLLATE latin1_bin DEFAULT NULL,
  `Straße` varchar(250) COLLATE latin1_bin DEFAULT NULL,
  `PLZ` varchar(10) COLLATE latin1_bin DEFAULT NULL,
  `Ort` varchar(100) COLLATE latin1_bin DEFAULT NULL,
  PRIMARY KEY (`LieferantId`),
  KEY `FK_Lieferanten_Users_UserId` (`UserId`),
  CONSTRAINT `FK_Lieferanten_Users_UserId` FOREIGN KEY (`UserId`) REFERENCES `nutzer` (`UserId`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=latin1 COLLATE=latin1_bin;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle vireq.nutzer
CREATE TABLE IF NOT EXISTS `nutzer` (
  `UserId` int(11) NOT NULL AUTO_INCREMENT,
  `UserName` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `Password` varchar(50) CHARACTER SET latin1 COLLATE latin1_bin NOT NULL,
  `LieferantenFile_LieferantenFileName` varchar(50) DEFAULT NULL,
  `LieferantenFile_LieferantenUpdatedAt` datetime DEFAULT NULL,
  `LieferantenFile_LieferantenNummerColumnNameFromCSVImport` varchar(50) DEFAULT NULL,
  `LieferantenFile_LieferantenNameColumnNameFromCSVImport` varchar(50) DEFAULT NULL,
  `LieferantenFile_LieferantenStraßeColumnNameFromCSVImport` varchar(50) DEFAULT NULL,
  `LieferantenFile_LieferantenPLZColumnNameFromCSVImport` varchar(50) DEFAULT NULL,
  `LieferantenFile_LieferantenOrtColumnNameFromCSVImport` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`UserId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- Daten Export vom Benutzer nicht ausgewählt

-- Exportiere Struktur von Tabelle vireq.paletten
CREATE TABLE IF NOT EXISTS `paletten` (
  `PaletteId` int(11) NOT NULL AUTO_INCREMENT,
  `ArtikelFile_ArtikelFileName` varchar(50) COLLATE latin1_bin DEFAULT NULL,
  `ArtikelFile_PaletteUpdatedAt` datetime DEFAULT NULL,
  `ArtikelFile_ArtikelnummerColumnNameFromCSVImport` varchar(50) COLLATE latin1_bin DEFAULT NULL,
  `ArtikelFile_ArtikelnameColumnNameFromCSVImport` varchar(50) COLLATE latin1_bin DEFAULT NULL,
  `LieferantId` int(11) DEFAULT NULL,
  PRIMARY KEY (`PaletteId`),
  KEY `FK_Palette` (`LieferantId`),
  CONSTRAINT `FK_palette_lieferanten` FOREIGN KEY (`LieferantId`) REFERENCES `lieferanten` (`LieferantId`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=23 DEFAULT CHARSET=latin1 COLLATE=latin1_bin;

-- Daten Export vom Benutzer nicht ausgewählt

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
