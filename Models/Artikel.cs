using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppl.Models
{
    [Table("artikel")]
    public class Artikel
    {
        /// <summary>
        /// eindeutige Id des Artikels
        /// </summary>
        public int ArtikelId { get; set; }

        /// <summary>
        /// Referenz auf die Palette, zu der dieser Artikel gehört
        /// </summary>
        public int PaletteId { get; set; }

        public string Artikelnummer { get; set; }
        public string Artikelname { get; set; }


        
    }
}