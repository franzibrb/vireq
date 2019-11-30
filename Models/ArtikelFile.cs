using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppl.Models
{
    [ComplexType]
    public class ArtikelFile
    {

        /// <summary>
        /// der Name der Artikeldatei
        /// </summary>
        public string ArtikelFileName { get; set; }

        /// <summary>
        /// Zeitpunkt, an welchem die Artikeldatei zuletzt aktualisiert wurden
        /// </summary>
        public DateTime? PaletteUpdatedAt { get; set; }



        /// <summary>
        /// Spaltenname der Artikelnummer
        /// </summary>
        public string ArtikelnummerColumnNameFromCSVImport { get; set; }


        /// <summary>
        /// Spaltenname des Artikelnamens
        /// </summary>
        public string ArtikelnameColumnNameFromCSVImport { get; set; }
    }

}