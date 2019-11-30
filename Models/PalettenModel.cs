using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppl.Models
{
    /// <summary>
    /// Kapselt die Paletten eines Lieferanten
    /// </summary>
    public class PalettenModel
    {


        public int LieferantenId { get; set; }
        public List<Palette> Paletten { get; set; }

      

    }
}