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
        public DbSet<PortfolioComposition> Portfolio { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<LastConnectionDB> DbConnections { get; set; }

        public qpcptfaw()
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<qpcptfaw>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().HasKey(t => new { t.AssetDBId, t.date });
        }

    }
}
