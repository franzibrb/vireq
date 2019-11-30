using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using MySql.Data.Entity;
using log4net;

namespace WebAppl.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ApplicationDbContext : DbContext
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationDbContext));

        public ApplicationDbContext() : base("DefaultConnection")
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Lieferant> Lieferanten { get; set; }
        public DbSet<Palette> Paletten { get; set; }
        public DbSet<Artikel> Artikel { get; set; }

        /// <summary>
        /// Liefert die Lieferanten für den Nutzer mit der übergebenen UserId. 
        /// 
        /// </summary>
        /// <param name="UserId">Id des Nutzers, für den Lieferanten abgefragt werden sollen</param>
        /// <returns>Lieferanten für den übergebenen Nutzer oder null im Fehlerfall </returns>
        public IEnumerable<Lieferant> GetLieferantenForUserWithId(int UserId)
        {
            IEnumerable<Lieferant> lieferanten = new List<Lieferant>();
            try
            {
                lieferanten =
                      (from lieferant in Lieferanten
                       where lieferant.UserId == UserId
                       select lieferant);

                lieferanten.ToList().ForEach(l => l.Paletten = GetPalettenForLieferantWithId(l.LieferantId)?.ToList());
                return lieferanten;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Liefert die Paletten für den Lieferanten mit der übergebenen LieferantenId. 
        /// 
        /// </summary>
        /// <param name="LieferantenId">Id des Lieferanten, für den Paltten abgefragt werden sollen</param>
        /// <returns>Liste von Paletten für den übergebenen LieferantenId oder null im Fehlerfall </returns>
        public IEnumerable<Palette> GetPalettenForLieferantWithId(int LieferantenId)
        {
            IEnumerable<Palette> paletten = new List<Palette>();
            try
            {
                paletten =
                      (from p in Paletten
                       where p.LieferantId == LieferantenId
                       select p);

                paletten.ToList().ForEach(p => p.Artikel = GetArtikelForPaletteWithId(p.PaletteId)?.ToList());
                return paletten;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public Palette GetPaletteById(int PalettenId)
        {
            try
            {
                var palette =
                      (from p in Paletten
                       where p.PaletteId == PalettenId
                       select p).FirstOrDefault();

                palette.Artikel = GetArtikelForPaletteWithId(PalettenId)?.ToList();
                return palette;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Liefert die Artikel für die Palette mit der übergebenen PalettenId. 
        /// 
        /// </summary>
        /// <param name="PalettenId">Id der Palette, für welche Artikel abgefragt werden sollen</param>
        /// <returns>Liste von Artikeln für die referenzierte Palette oder null im Fehlerfall </returns>
        public IEnumerable<Artikel> GetArtikelForPaletteWithId(int PalettenId)
        {
            IEnumerable<Artikel> artikel = new List<Artikel>();
            try
            {
                artikel =
                      (from a in Artikel
                       where a.PaletteId == PalettenId
                       select a);

                return artikel;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Liefert den Lieferanten mit der übergebenen LieferantenId. 
        /// </summary>
        /// <param name="LieferantId">Id des Lieferanten, der abgefragt werden soll</param>
        /// <returns>Lieferant mit der übergebenen Id oder null im Fehlerfall</returns>
        public Lieferant GetLieferantById(int LieferantId)
        {
            try
            {
                var lieferant = (from lief in Lieferanten
                                 where lief.LieferantId == LieferantId
                                 select lief).FirstOrDefault();
                return lieferant;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }


        /// <summary>
        /// Liefert den Lieferanten mit der übergebenen LieferantenId. 
        /// </summary>
        /// <param name="LieferantId">Id des Lieferanten, der abgefragt werden soll</param>
        /// <returns>Lieferant mit der übergebenen Id oder null im Fehlerfall</returns>
        public Lieferant GetLieferantByLieferantennummer(int lieferantenNummer)
        {
            try
            {
                var lieferant = (from lief in Lieferanten
                                 where lief.Lieferantennummer == lieferantenNummer
                                 select lief).First();
                return lieferant;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Liefert den Nutzer mit der übergebenen UserId. 
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns>Nutzer mit der übergebenen Id oder null im Fehlerfall</returns>
        public User GetUserById(int UserId)
        {
            try
            {
                var user = (from us in Users
                            where us.UserId == UserId
                            select us).FirstOrDefault();
                return user;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Aktualisiert die Daten der Lieferantendatei für den Nutzer mit der übergebenen UserId.
        /// </summary>
        /// <param name="UserId">die Id des Nutzers, für welchen ein Lieferant aktualisiert werden soll</param>
        /// <param name="LieferantenFile">das zu aktualisierende File Objekt</param>
        /// <returns></returns>
        public bool UpdateLieferantenDateiForUser(int UserId, LieferantenFile LieferantenFile)
        {
            bool success = false;
            try
            {
                // Nutzer holen
                var user = (from u in Users where u.UserId == UserId select u).FirstOrDefault();

                if (user == null)
                {
                    success = false;
                    Log.Error("Nutzer mit UserId " + UserId + " nicht in DB. Aktualisieren der Lieferanten nicht möglich.");
                }
                else
                {
                    user.LieferantenFile = LieferantenFile;
                    success = SaveChanges() == 1;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }
            return success;
        }

        /// <summary>
        /// Aktualisiert den übergebenen Lieferanten mit der Lieferantennummer und der NutzerId.
        /// Exisitert der Lieferant noch nicht in der DB, wird er neu eingefügt.
        /// </summary>
        /// <param name="Lieferant"></param>
        /// <returns></returns>
        public void UpdateLieferant(Lieferant Lieferant)
        {

            try
            {

                var lieferant = (from l in Lieferanten
                                 where l.Lieferantennummer == Lieferant.Lieferantennummer && l.UserId == Lieferant.UserId
                                 select l).FirstOrDefault();

                if (lieferant == null)
                {
                    Lieferant newLieferant = Lieferanten.Create();
                    newLieferant = Lieferant;
                    Lieferanten.Add(newLieferant);
                    if (SaveChanges() == 0)
                    {
                        Log.Info("Lieferant  " + newLieferant.Lieferantennummer + " für Nutzer " + newLieferant.UserId + " wurde nicht hinzugefügt.");
                    }

                }
                else
                {
                    Lieferanten.Attach(lieferant);
                    lieferant.Lieferantenname = Lieferant.Lieferantenname;
                    lieferant.Straße = Lieferant.Straße;
                    lieferant.Ort = Lieferant.Ort;
                    lieferant.PLZ = Lieferant.PLZ;
                    if (SaveChanges() == 0)
                    {
                        Log.Info("Lieferant  " + Lieferant.Lieferantennummer + " für Nutzer " + Lieferant.UserId + " wurde nicht aktualisiert.");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }

        }



        /// <summary>
        /// Löscht den übergebenen Lieferanten.
        /// </summary>
        /// <param name="Lieferant"></param>
        /// <returns></returns>
        public void DeleteLieferant(Lieferant Lieferant)
        {
            try
            {
                if (Lieferant != null)
                {
                    Lieferanten.Remove(Lieferant);
                    if (SaveChanges() == 0)
                    {
                        Log.Error("Lieferant  " + Lieferant.Lieferantennummer + " für Nutzer " + Lieferant.UserId + " wurde nicht gelöscht.");
                    }

                }
                else
                {
                    Log.Error("Lieferant  " + Lieferant.Lieferantennummer + " für Nutzer " + Lieferant.UserId + " wurde nicht gefunden und konnte nicht gelöscht werden.");
                }

            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }

        }




        public int UpdateArtikelDateiForLieferantAndPalette(int LieferantId, int PalettenId, ArtikelFile ArtikelFile)
        {
            int retVal = -1;
            try
            {
                // Lieferanten holen
                var lieferant = (from l in Lieferanten where l.LieferantId == LieferantId select l).FirstOrDefault();

                if (lieferant == null)
                {
                    Log.Error("Lieferant mit LieferantId " + LieferantId + " nicht in DB. Aktualisieren der Artikel nicht möglich.");
                }
                else
                {
                    // Palette holen
                    var palette = (from p in Paletten where p.PaletteId == PalettenId && p.LieferantId == LieferantId select p).FirstOrDefault();

                    // noch keine da --> neu anlegen
                    if (palette == null)
                    {
                        Palette added = Paletten.Add(new Palette()
                        {
                            LieferantId = LieferantId,
                            ArtikelFile = ArtikelFile

                        });
                        if (SaveChanges() == 1)
                        {
                            retVal = added.PaletteId;
                        }
                        else
                        {
                            Log.Error("Hinzufügen einer neuen Palette für Lieferant mit LieferantId " + LieferantId + " nicht erfolgreich");

                        }

                    }
                    // eine da --> aktualisieren
                    else
                    {
                        palette.ArtikelFile = ArtikelFile;
                        if (SaveChanges() == 1)
                        {
                            retVal = palette.PaletteId;
                        }
                        else
                        {
                            Log.Error("Aktualisieren der Palette " + PalettenId + "  für Lieferant mit LieferantId " + LieferantId + " nicht erfolgreich");

                        }
                    }


                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }
            return retVal;
        }

        public Artikel GetArtikelById(int ArtikelId)
        {
            try
            {
                var artikel =
                      (from a in Artikel
                       where a.ArtikelId == ArtikelId
                       select a).FirstOrDefault();


                return artikel;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return null;
            }
        }

        public void UpdateArtikel(Artikel Artikel)
        {

            try
            {

                var artikel = (from a in this.Artikel
                               where a.PaletteId == Artikel.PaletteId && a.Artikelnummer == Artikel.Artikelnummer
                               select a).FirstOrDefault();

                if (artikel == null)
                {
                    Artikel newArtikel = this.Artikel.Create();
                    newArtikel = Artikel;
                    this.Artikel.Add(newArtikel);
                    if (SaveChanges() == 0)
                    {
                        Log.Info("Artikel  " + newArtikel.Artikelnummer + " für Palette " + newArtikel.PaletteId + " wurde nicht hinzugefügt.");
                    }

                }
                else
                {
                    this.Artikel.Attach(artikel);
                    artikel.Artikelname = Artikel.Artikelname;
                    if (SaveChanges() == 0)
                    {
                        Log.Info("Artikel  " + artikel.Artikelnummer + " für Palette " + artikel.PaletteId + " wurde nicht aktualisiert.");

                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);

            }

        }

    }
}
