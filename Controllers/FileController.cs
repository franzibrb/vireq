﻿using System;
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
    public class FileController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileController));


        /// <summary>
        /// POST /File/UploadLieferantenFile
        /// Verarbeitet die Lieferanten CSV Datei und aktualisiert die Lieferanten für den angemeldeten Nutzer
        /// </summary>
        /// <returns>HttpStatusCodeResult welches beschreibt, ob die Verarbeitung und die Aktualisierung in der DB erfolgreich war</returns>
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
                                    //trenne bei ; wenn nicht innerhalb von Hochkommata
                                    string pattern = ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                                    string[] lineArr = Regex.Split(line, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
                                    // Spaltenanzahl zu gering
                                    if (lineArr.Length < 5)
                                    {
                                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Die Datei muss mindestens 5 Spalten enthalten. Dies ist" +
                                            "nicht der Fall für Zeile " + lineCount + ".");

                                    }
                                    // Header verarbeiten
                                    if (lineCount == 0)
                                    {
                                        LieferantenFile lieferantenFile = new LieferantenFile()
                                        {
                                            LieferantenFileName = file.FileName,
                                            LieferantenNummerColumnNameFromCSVImport = lineArr[0],
                                            LieferantenNameColumnNameFromCSVImport = lineArr[1],
                                            LieferantenStraßeColumnNameFromCSVImport = lineArr[2],
                                            LieferantenPLZColumnNameFromCSVImport = lineArr[3],
                                            LieferantenOrtColumnNameFromCSVImport = lineArr[4],
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
                                    // Lieferanten verarbeiten
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


        /// <summary>
        /// GET /File/DownloadLieferantenFile
        /// Exportiert die Lieferanten des aktuellen Nutzers in eine CSV Datei 
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize]
        [HttpGet]
        public ActionResult DownloadLieferantenFile()
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    int userId = ((CustomPrincipal)User).UserId;
                    List<Lieferant> Lieferanten = new List<Lieferant>();
                    LieferantenFile LieferantenFile = new LieferantenFile();

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {

                        IEnumerable<Lieferant> lieferanten = context.GetLieferantenForUserWithId(userId);

                        if (lieferanten != null)
                        {
                            Lieferanten = lieferanten.ToList();
                        }

                        //wenn es Lieferanten gibt, sollten die anderen Felder ebenfalls befüllt sein
                        User user = context.GetUserById(userId);
                        if (user.LieferantenFile != null)
                        {
                            LieferantenFile = user.LieferantenFile;
                        }
                    }



                    StringWriter sw = new StringWriter();
                    sw.WriteLine("{0};{1};{2};{3};{4}",
                        LieferantenFile.LieferantenNummerColumnNameFromCSVImport,
                        LieferantenFile.LieferantenNameColumnNameFromCSVImport,
                        LieferantenFile.LieferantenStraßeColumnNameFromCSVImport,
                        LieferantenFile.LieferantenPLZColumnNameFromCSVImport,
                        LieferantenFile.LieferantenOrtColumnNameFromCSVImport);

                    Lieferanten.ForEach(lief =>
                    {
                        sw.WriteLine("{0};{1};{2};{3};{4}",
                            lief.Lieferantennummer,
                            surroundWithDoubleQuotesIfContainsBlanks(lief.Lieferantenname),
                            surroundWithDoubleQuotesIfContainsBlanks(lief.Straße),
                            lief.PLZ,
                            surroundWithDoubleQuotesIfContainsBlanks(lief.Ort));
                    });

                    return File(System.Text.Encoding.GetEncoding(1252).GetBytes(sw.ToString()), "text/csv", LieferantenFile.LieferantenFileName);

                }
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Erstellung der Export-Datei ist ein Fehler aufgetreten.");
            }

        }


        /// <summary>
        /// POST /File/UploadPalette
        /// Verarbeitet eine Artikel CSV Datei und aktualisiert die Artikel der Palette mit der übergebenen palettenId 
        /// Wenn die Palette noch nicht existiert, wird sie neu erstellt. 
        /// Wenn ein Artikel innerhalb der Palette noch nicht existiert, wird er neu erstellt. 

        /// </summary>
        /// <returns>HttpStatusCodeResult welches beschreibt, ob die Verarbeitung und die Aktualisierung in der DB erfolgreich war</returns>
        [CustomAuthorize]
        [HttpPost]
        public ActionResult UploadPalette(int? palettenId)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int userId = ((CustomPrincipal)(User)).UserId;


                    if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        string fileName = file.FileName;
                        if (fileName.EndsWith(".csv"))
                        {
                            // gibt es den Lieferanten überhaupt?
                            string lieferantenNummer = fileName.Remove(fileName.Length - 4);
                            Lieferant lieferant = null;
                            using (ApplicationDbContext context = new ApplicationDbContext())
                            {
                                lieferant = context.GetLieferantByLieferantennummer(int.Parse(lieferantenNummer));
                                if (lieferant == null) return new HttpNotFoundResult("Lieferant mit Nummer " + lieferantenNummer + " nicht gefunden. Hochladen der Artikeldatei nicht möglich.");
                            }
                            Stream stream = file.InputStream;
                            using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding(1252)))
                            {
                                int lineCount = 0;
                                string line = string.Empty;
                                while ((line = reader.ReadLine()) != null)
                                {
                                    //trenne bei ; wenn nicht innerhalb von Hochkommata
                                    string pattern = ";(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";
                                    string[] lineArr = Regex.Split(line, pattern, RegexOptions.None, TimeSpan.FromSeconds(1));
                                    // Spaltenanzahl zu gering
                                    if (lineArr.Length < 2)
                                    {
                                        return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest, "Die Datei muss mindestens 2 Spalten enthalten. Dies ist" +
                                            "nicht der Fall für Zeile " + lineCount + ".");

                                    }
                                    // Header verarbeiten
                                    if (lineCount == 0)
                                    {
                                        ArtikelFile artikelFile = new ArtikelFile()
                                        {
                                            ArtikelFileName = file.FileName,
                                            ArtikelnummerColumnNameFromCSVImport = lineArr[0],
                                            ArtikelnameColumnNameFromCSVImport = lineArr[1],
                                            PaletteUpdatedAt = DateTime.Now

                                        };
                                        using (ApplicationDbContext context = new ApplicationDbContext())
                                        {
                                            palettenId = context.UpdatePalette(lieferant.LieferantId, palettenId.HasValue ? palettenId.Value : -1, artikelFile);
                                            if (palettenId < 0)
                                            {
                                                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Fehler beim Schreiben in die DB. Bitte prüfen Sie die Log-Datei");
                                            }
                                        }
                                    }
                                    // Artikel verarbeiten                                 
                                    else
                                    {
                                        Artikel artikel = new Artikel()
                                        {
                                            //TODO remove leading and trailing ""
                                            PaletteId = palettenId.Value,
                                            Artikelnummer = lineArr[0],
                                            Artikelname = lineArr[1]
                                        };
                                        using (ApplicationDbContext context = new ApplicationDbContext())
                                        {
                                            context.UpdateArtikel(artikel);

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



        /// <summary>
        /// GET /File/DownloadPalette
        /// Exportiert die Palette mit der übergebenen Id 
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize]
        [HttpGet]
        public ActionResult DownloadPalette(int palettenId)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {

                    Palette Palette = new Palette();

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        Palette = context.GetPaletteById(palettenId);


                    }



                    StringWriter sw = new StringWriter();
                    sw.WriteLine("{0};{1}",
                        Palette.ArtikelFile.ArtikelnummerColumnNameFromCSVImport,
                        Palette.ArtikelFile.ArtikelnameColumnNameFromCSVImport
                    );

                    Palette.Artikel.ForEach(artikel =>
                    {
                        sw.WriteLine("{0};{1}",
                            artikel.Artikelnummer,
                            surroundWithDoubleQuotesIfContainsBlanks(artikel.Artikelname)
                        );
                    });

                    return File(System.Text.Encoding.GetEncoding(1252).GetBytes(sw.ToString()), "text/csv", Palette.ArtikelFile.ArtikelFileName);

                }
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Unauthorized);

            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Bei der Erstellung der Export-Datei ist ein Fehler aufgetreten.");
            }

        }


        #region Helper
        /// <summary>
        /// Packt den übergebenen string in Hochkommata, wenn Leerzeichen enthalten sind
        /// </summary>
        /// <returns>Zeichenkette in Anführungszeichen, wenn Leerzeichen enthalten sind und die Zeichenkette nicht sowieso von diesen umschlossen ist; sonst Originalzeichenkette</returns>
        private string surroundWithDoubleQuotesIfContainsBlanks(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            if (input.Contains(" ") && !input.StartsWith("\"") && !input.EndsWith("\""))
                return "\"" + input + "\"";
            return input;
        }
        #endregion



    }
}