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
    public class HomeController : Controller
    {

        private static readonly ILog Log = LogManager.GetLogger(typeof(HomeController));



        // GET: Home
        [CustomAuthorize]
        public ActionResult Index(HomeModel model)
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
                        IEnumerable<Lieferant> lieferanten = (from lieferant in context.Lieferanten
                                                              where lieferant.UserId == userId
                                                              select lieferant);
                        if (lieferanten != null)
                        {
                            model.Lieferanten = lieferanten.ToList();
                        }

                        //wenn es Lieferanten gibt, sollten die anderen Felder ebenfalls befüllt sein
                        User user = (from us in context.Users
                                     where us.UserId == userId
                                     select us).FirstOrDefault();
                        if (user.LieferantenFile != null)
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
            return View(model);
        }

    }

}