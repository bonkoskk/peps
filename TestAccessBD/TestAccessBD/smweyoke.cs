using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TestAccessBD
{
    public class smweyoke : DbContext
    {
        public DbSet<AssetDB> Assets { get; set; }
        public DbSet<PortfolioComposition> Portfolio { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<LastConnectionDB> DbConnections { get; set; }

        public smweyoke()
            : base("smweyoke")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Price>().HasKey(t => new { t.AssetDBId, t.date });
        }

    }
}
