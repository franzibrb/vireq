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



        // GET: Lieferanten
        [CustomAuthorize]
        public ActionResult Index(LieferantenModel model)
        {
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
                ModelState.AddModelError("lieferantenabfrage", "Es ist ein Fehler bei der Abfrage Ihrer Kunden aufgetreten.");
            }
            return PartialView("~/Views/Lieferanten/_Lieferanten.cshtml", model);

        }

        // GET: Lieferant/Details
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

        // POST: Lieferant/Details
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