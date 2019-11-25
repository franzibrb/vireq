using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Text.RegularExpressions;
using log4net;
using System.IO;
using WebAppl.Models;
using WebAppl.CustomAuthentication;

namespace WebAppl.Controllers
{
    public class UploadController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(UploadController));

        // GET: Upload
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FileUpload"></param>
        /// <returns></returns>
        [CustomAuthorize]
        [HttpPost]
        public ActionResult UploadLieferantenFile()
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int userId = ((CustomPrincipal)(User)).UserId;

                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file.FileName.EndsWith(".csv"))
                        {
                            Stream stream = file.InputStream;
                            bool success = false;
                            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding(1252)))
                            {
                                int lineCount = 0;
                                string line = string.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    string pattern = ";(?=(?:[^']*'[^']*')*[^']*$)";
                                    string[] lineArr = Regex.Split(line, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
                                    // Header verarbeiten
                                    if (lineCount == 0)
                                    {
                                        LieferantenFile lieferantenFile = new LieferantenFile()
                                        {
                                            LieferantenFileName = file.FileName,
                                            LieferantenNummerColumnNameFromCSVImport = lineArr[0],
                                            LieferantenNameColumnNameFromCSVImport = lineArr[1],
                                            LieferantenUpdatedAt = DateTime.Now
                                        };
                                        using (ApplicationDbContext context = new ApplicationDbContext())
                                        {
                                            success = context.UpdateLieferantenDateiForUser(userId, lieferantenFile);
                                            if (!success)
                                            {
                                                Log.Error("UpdateLieferantenDateiForUser nicht erfolgreich für Nutzer " + userId);
                                                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Fehler beim Schreiben in die DB. Bitte prüfen Sie die Log-Datei");
                                            }
                                        }
                                    }
                                    // Lieferanten
                                    else
                                    {
                                        Lieferant lieferant = new Lieferant()
                                        {
                                            //TODO remove leading and trailing ""
                                            UserId = userId,
                                            Lieferantennummer = int.Parse(lineArr[0]),
                                            Lieferantenname = lineArr[1],
                                            Straße = lineArr[2],
                                            PLZ = int.Parse(lineArr[3]),
                                            Ort = lineArr[4],
                                        };
                                        using (ApplicationDbContext context = new ApplicationDbContext())
                                        {
                                            context.UpdateLieferant(lieferant);
                                        
                                        }

                                    }
                                    lineCount++;
                                }


                            }

                        }
                        else
                        {
                            return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Das Dateiformat wird nicht unterstützt.");
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Bitte wählen Sie eine Datei aus.");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Verarbeitung der Import-Datei ist ein Fehler aufgetreten.");
            }
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

        }
    }

}