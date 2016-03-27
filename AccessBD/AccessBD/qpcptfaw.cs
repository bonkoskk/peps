using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace AccessBD
{
    public class qpcptfaw : DbContext
    {
        public DbSet<AssetDB> Assets { get; set; }
        public DbSet<HedgingPortfolio> Portfolio { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<LastConnectionDB> DbConnections { get; set; }
        public DbSet<EvergladesDB> Everglades { get; set; }
        public DbSet<CashDB> Cash { get; set; }
        public DbSet<RateDB> InteresRatesType { get; set; }
        public DbSet<RateDBValue> Rates { get; set; }
        public DbSet<CovDB> Covariance { get; set; }
        public DbSet<PortfolioComposition> PortCompositions { get; set; }

        public qpcptfaw()
        {
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<qpcptfaw>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().HasKey(t => new { t.AssetDBId, t.date });
            modelBuilder.Entity<RateDBValue>().HasKey(t => new { t.RateDBId, t.date });
            modelBuilder.Entity<PortfolioComposition>().HasKey(t => new { t.AssetDBId, t.date });
            modelBuilder.Entity<CovDB>().HasKey(t => new { t.date, t.indexX, t.indexY });
        }

    }
}
