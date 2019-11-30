using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppl.Models
{
    /// <summary>
    /// Kapselt die Artikel einer Palette
    /// </summary>
    public class ArtikelModel
    {


        public Palette Palette { get; set; }

        public List<Artikel> Artikel { get; set; }



    }
}