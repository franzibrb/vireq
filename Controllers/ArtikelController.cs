using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppl.Models;
using WebAppl.CustomAuthentication;
using log4net;

namespace WebAppl.Controllers
{
    public class ArtikelController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ArtikelController));


        // GET: Artikel/ByPalette/{palettenId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult ByPalette(int palettenId)
        {

            return PartialView("~/Views/Artikel/_Artikel.cshtml", palettenId);


        }

        // GET: Artikel/IndexGrid/{palettenId}
        [CustomAuthorize]
        [HttpGet]
        public PartialViewResult IndexGrid(int palettenId)
        {
            ArtikelModel model = new ArtikelModel();
            try
            {
                model.Artikel = new List<Artikel>();
                model.Palette = new Palette();

                //Artikel abfragen
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    IEnumerable<Artikel> artikel = context.GetArtikelForPaletteWithId(palettenId);
                    if (artikel != null)
                    {
                        model.Artikel = artikel.ToList();
                    }

                    Palette palette = context.GetPaletteById(palettenId);
                    if (palette != null)
                    {
                        model.Palette = palette;
                    }

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }
            return PartialView("~/Views/Artikel/_ArtikelGrid.cshtml", model);


        }



        // GET: Artikel/Details/{artikelId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Details(int artikelId)
        {
            try
            {
                Artikel artikel = null;

                //Artikel abfragen
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    artikel = context.GetArtikelById(artikelId);
                    if (artikel != null)
                    {
                        return PartialView("~/Views/Artikel/_ArtikelDetail.cshtml", artikel);

                    }
                    else
                    {
                        return new HttpNotFoundResult("Artikel mit Id " + artikelId + " nicht gefunden");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Abfrage des Artikels mit Id " + artikelId + " ist ein Fehler aufgetreten.");

            }

        }

        // POST: Artikel/Details
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Details(Artikel Artikel)
        {
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    context.UpdateArtikel(Artikel);

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Beim Aktualiseren des Artikels mit Id " + Artikel.ArtikelId + " ist ein Fehler aufgetreten.");

            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

        }

    }
}