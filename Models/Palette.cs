using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebAppl.Models
{
    [Table("paletten")]
    public class Palette
    {

        /// <summary>
        /// eindeutige Id der Palette
        /// </summary>
        public int PaletteId { get; set; }

        /// <summary>
        /// Referenz auf den Lieferanten, zu dem diese Palette gehört
        /// </summary>
        public int LieferantId { get; set; }

        /// <summary>
        /// die Datei, durch welche die Artikel hochgeladen wurden
        /// </summary>
        public ArtikelFile ArtikelFile { get; set;}

        public List<Artikel> Artikel { get; set; }
    }
}