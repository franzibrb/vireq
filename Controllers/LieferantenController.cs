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
    public class LieferantenController : Controller
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(LieferantenController));


        /// <summary>
        /// Fragt die Infos des Lieferanten-Imports für den gerade angemeldeten Benutzer ab, 
        /// kapselt diese in ein LieferantenFile und gibt dieses an die View weiter.
        /// </summary>
        /// <param name="LieferantenFile"></param>
        /// <returns>PartialView mit LieferantenFile</returns>
        // GET: Lieferanten
        [CustomAuthorize]
        public ActionResult Index(LieferantenFile LieferantenFile)
        {
            try
            {

                LieferantenFile = new LieferantenFile();

                //LieferantenFile abfragen
                if (User != null && User.Identity.IsAuthenticated)
                {
                    int userId = ((CustomPrincipal)User).UserId;

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {

                        User user = context.GetUserById(userId);
                        if (user?.LieferantenFile != null)
                        {
                            LieferantenFile = user.LieferantenFile;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                ModelState.AddModelError("lieferantenabfrage", "Es ist ein Fehler bei der Abfrage Ihrer Kunden aufgetreten.");
            }
            return PartialView("~/Views/Lieferanten/_Lieferanten.cshtml", LieferantenFile);

        }

        /// <summary>
        /// Fragt die Lieferanten des angemeldeten Nutzers ab und packt diese in ein LieferantenModel.
        /// Dieses wird an die View weitergereicht.
        /// </summary>
        /// <returns>PartialView mit LieferantenModel</returns>
        // GET: Lieferanten/IndexGrid
        [CustomAuthorize]
        public PartialViewResult IndexGrid()
        {
            LieferantenModel model = new LieferantenModel();

            try
            {
                model.Lieferanten = new List<Lieferant>();
                model.LieferantenFile = new LieferantenFile();


                //Lieferanten abfragen
                if (User != null && User.Identity.IsAuthenticated)
                {
                    int userId = ((CustomPrincipal)User).UserId;

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        IEnumerable<Lieferant> lieferanten = context.GetLieferantenForUserWithId(userId);
                        if (lieferanten != null)
                        {
                            model.Lieferanten = lieferanten.ToList();
                        }

                        //wenn es Lieferanten gibt, sollten die anderen Felder ebenfalls befüllt sein
                        User user = context.GetUserById(userId);
                        if (user?.LieferantenFile != null)
                        {
                            model.LieferantenFile = user.LieferantenFile;
                        }
                    }
                }



            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return PartialView("~/Views/Lieferanten/_LieferantenGrid.cshtml", model);

        }

        /// <summary>
        /// Fragt den Lieferanten mit der übergebenen Id ab
        /// </summary>
        /// <param name="lieferantenId">Id des abzufragenden Lieferanten</param>
        /// <returns>PartialView mit Lieferant; HttpNotFoundResult, wenn der Lieferant nicht gefunden wurde oder  HttpStatusCodeResult 500 im Fehlerfall</returns>
        // GET: Lieferanten/Details/{lieferantenId}
        [CustomAuthorize]
        [HttpGet]
        public ActionResult Details(int lieferantenId)
        {
            try
            {
                Lieferant lieferant = null;

                //Lieferant abfragen
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    lieferant = context.GetLieferantById(lieferantenId);
                    if (lieferant != null)
                    {
                        return PartialView("~/Views/Lieferanten/_LieferantenDetail.cshtml", lieferant);

                    }
                    else
                    {
                        return new HttpNotFoundResult("Lieferant mit Id " + lieferantenId + " nicht gefunden");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Abfrage des Lieferanten mit Id " + lieferantenId + " ist ein Fehler aufgetreten.");

            }

        }

        /// <summary>
        /// Aktualisiert den übergebenen Lieferanten
        /// </summary>
        /// <param name="Lieferant">der zu aktualisierende Lieferant</param>
        /// <returns>HttpStatusCodeResult 200 oder HttpStatusCodeResult 500 im Fehlerfall</returns>
        // POST: Lieferanten/Details
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Details(Lieferant Lieferant)
        {
            try
            {
                using (ApplicationDbContext context = new ApplicationDbContext())
                {
                    context.UpdateLieferant(Lieferant);

                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Beim Aktualiseren des Lieferanten mit Id " + Lieferant.LieferantId + " ist ein Fehler aufgetreten.");

            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

        }

        /// <summary>
        /// Lösche die Lieferanten mit den übergebenen Ids
        /// </summary>
        /// <param name="lieferantenIds">die zu löschenden LieferantenIds</param>
        /// <returns>HttpStatusCodeResult 200 oder HttpStatusCodeResult 500 im Fehlerfall</returns>
        // POST /Lieferanten/Delete
        [CustomAuthorize]
        [HttpPost]
        public ActionResult Delete(string[] lieferantenIds)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    int userId = ((CustomPrincipal)User).UserId;

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        IEnumerable<Lieferant> lieferanten = new List<Lieferant>();

                        for (int i = 0; i < lieferantenIds.Length; i++)
                        {
                            Lieferant lieferant = context.GetLieferantById(int.Parse(lieferantenIds[i]));
                            if (lieferant != null)
                            {
                                context.DeleteLieferant(lieferant);
                            }
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