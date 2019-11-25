using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using MySql.Data.Entity;
using log4net;

namespace WebAppl.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class ApplicationDbContext : DbContext
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ApplicationDbContext));

        public ApplicationDbContext() : base("DefaultConnection") //Connection string name write here  
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Lieferant> Lieferanten { get; set; }



        /// <summary>
        /// Aktualisiert die Daten der Lieferantendatei für den Nutzer mit der übergebenen UserId.
        /// </summary>
        /// <param name="UserId">die Id des Nutzers, für welchen ein Lieferant aktualisiert werden soll</param>
        /// <param name="Lieferant"></param>
        /// <returns></returns>
        public bool UpdateLieferantenDateiForUser(int UserId, LieferantenFile LieferantenFile)
        {
            bool success = false;
            try
            {
                //aktuellen Nutzer holen
                var user = (from u in this.Users where u.UserId == UserId select u).FirstOrDefault();
                //Fehler, ein Nutzer muss allein schon durch seine Registrierung in der DB sein
                if (user == null)
                {
                    success = false;
                    Log.Error("Nutzer mit UserID " + UserId + " nicht in DB. Aktualisieren der Lieferanten nicht möglich.");
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
        /// Aktualisiert den übergebenen Lieferanten.
        /// Exisitert der Lieferant noch nicht, wird er neu eingefügt.
        /// </summary>
        /// <param name="Lieferant"></param>
        /// <returns></returns>
        public void UpdateLieferant(Lieferant Lieferant)
        {
            bool success = false;
            try
            {
                var lieferant = (from l in this.Lieferanten
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
                    Lieferant.LieferantId = lieferant.LieferantId;
                    this.Entry(lieferant).CurrentValues.SetValues(Lieferant);
                    if(SaveChanges()== 0)
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

    }
}