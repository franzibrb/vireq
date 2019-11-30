using System;

using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using log4net;
using System.Text.RegularExpressions;
using WebAppl.CustomAuthentication;
using WebAppl.Models;

namespace WebAppl.Controllers
{
    public class PalettenController : Controller
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(PalettenController));

        // GET: Paletten/ByLieferant/{lieferantenId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult ByLieferant(int lieferantenId)
        {
            
            return PartialView("~/Views/Paletten/_Paletten.cshtml", lieferantenId);


        }

        // GET: Paletten/IndexGrid/{lieferantenId}
        [CustomAuthorize]
        [HttpGet]
        public PartialViewResult IndexGrid(int lieferantenId)
        {
            PalettenModel model = new PalettenModel();
            model.LieferantenId = lieferantenId;
            try
            {

                //Paletten abfragen
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    IEnumerable<Palette> paletten = context.GetPalettenForLieferantWithId(lieferantenId);
                    if (paletten != null)
                    {
                        model.Paletten = paletten.ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }
            return PartialView("~/Views/Paletten/_PalettenGrid.cshtml", model);


        }

        // GET: Paletten/Details/{palettenId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Details(int palettenId)
        {
            try
            {
                Palette palette = null;

                //Palette abfragen
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    // palette = context.GetPaletteById(palettenId);
                    if (palette != null)
                    {
                        return PartialView("~/Views/Paletten/_PalettenDetail.cshtml", palette);

                    }
                    else
                    {
                        return new HttpNotFoundResult("Palette mit Id " + palettenId + " nicht gefunden");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Abfrage der Palette mit Id " + palettenId + " ist ein Fehler aufgetreten.");

            }

        }
    }

}