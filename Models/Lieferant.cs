using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppl.Models
{
    [Table("lieferanten")]
    public class Lieferant
    {
        /// <summary>
        /// eindeutige Id des Lieferanten
        /// </summary>
        public int LieferantId { get; set; }

        /// <summary>
        /// Referenz auf den Nutzer, zu dem dieser Lieferant gehört
        /// </summary>
        public int UserId { get; set; }


        public int Lieferantennummer { get; set; }
        public string Lieferantenname { get; set; }
        public string Straße { get; set; }
        public int PLZ { get; set; }
        public string Ort { get; set; }

        /// <summary>
        /// Paletten dieses Lieferanten
        /// </summary>
        public List<Palette> Paletten { get; set; }
    }
}