using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppl.Models
{
    [ComplexType]
    public class LieferantenFile
    {

        /// <summary>
        /// der Name der Lieferantendatei, die dieser Nutzer eingelesen hat
        /// </summary>
        public string LieferantenFileName { get; set; }

        /// <summary>
        /// Zeitpunkt, an welchem die Lieferanten zuletzt aktualisiert wurden
        /// </summary>
        public DateTime? LieferantenUpdatedAt { get; set; }

        /// <summary>
        /// Spaltenname der Lieferantennummer
        /// </summary>
        public string LieferantenNummerColumnNameFromCSVImport { get; set; }

        /// <summary>
        /// Spaltenname des Lieferantennamens
        /// </summary>
        public string LieferantenNameColumnNameFromCSVImport { get; set; }


        /// <summary>
        /// Spaltenname der Straße
        /// </summary>
        public string LieferantenStraßeColumnNameFromCSVImport { get; set; }


        /// <summary>
        /// Spaltenname der PLZ
        /// </summary>
        public string LieferantenPLZColumnNameFromCSVImport { get; set; }

        /// <summary>
        /// Spaltenname des Ortes
        /// </summary>
        public string LieferantenOrtColumnNameFromCSVImport { get; set; }
    }
}