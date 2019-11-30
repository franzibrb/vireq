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

        /// <summary>
        /// Kapselt lediglich die übergebene PalettenId und gibt sie an die View weiter
        /// </summary>
        /// <param name="palettenId">die an die View zu übergebene PalettenId</param>
        /// <returns>PartialView mit übergebener PalettenId</returns>
        // GET: Artikel/ByPalette/{palettenId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult ByPalette(int palettenId)
        {

            return PartialView("~/Views/Artikel/_Artikel.cshtml", palettenId);


        }

        /// <summary>
        /// Fragt die Artikel der übergebenen PalettenId ab und packt diese in ein ArtikelModel.
        /// Dieses wird an die View weitergegeben.
        /// </summary>
        /// <param name="palettenId">PalettenId, für welche Artikel dargestellt werden sollen</param>
        /// <returns>PartialViewResult mit ArtikelModel</returns>
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


        /// <summary>
        /// Fragt den Artikel mit der übergebenen Id ab
        /// </summary>
        /// <param name="artikelId">Id des abzufragenden Artikels</param>
        /// <returns>PartialView mit Artikel; HttpNotFoundResult, wenn der Artikel nicht gefunden wurde oder HttpStatusCodeResult 500, wenn ein Fehler auftrat </returns>
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

        /// <summary>
        /// Aktualisiert den übergebenen Artikel
        /// </summary>
        /// <param name="Artikel">der zu aktualisierende Artikel</param>
        /// <returns>HttpStatusCodeResult 200 oder HttpStatusCodeResult 500 im Fehlerfall</returns>
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


        /// <summary>
        /// Löscht die Artikel mit den übergebenen Ids
        /// </summary>
        /// <param name="artikelIds">die zu löschenden ArtikelIds</param>
        /// <returns>HttpStatusCodeResult 200 oder HttpStatusCodeResult 500 im Fehlerfall</returns>
        // POST /Artikel/Delete
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Delete(string[] artikelIds)
        {
            try
            {

                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    IEnumerable<Artikel> artikel = new List<Artikel>();

                    for (int i = 0; i < artikelIds.Length; i++)
                    {
                        Artikel art = context.GetArtikelById(int.Parse(artikelIds[i]));
                        if (art != null)
                        {
                            context.DeleteArtikel(art);
                        }
                    }

                }



            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Beim Löschen der Lieferanten ist ein Fehler aufgetreten.");

            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

        }
    }
}
