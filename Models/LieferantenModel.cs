using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppl.Models
{
    /// <summary>
    /// Kapselt die Lieferanten und Lieferantendatei eines Nutzers
    /// </summary>
    public class LieferantenModel
    {



        public List<Lieferant> Lieferanten { get; set; }

        public LieferantenFile LieferantenFile { get; set; }
      

    }
}