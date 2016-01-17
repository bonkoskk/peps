using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TestAccessBD
{
    public class DatabasePeps : DbContext
    {
        public DbSet<AssetDB> Assets { get; set; }
        public DbSet<PortfolioComposition> Portfolio { get; set; }
        public DbSet<PortfolioValue> PortfolioValues { get; set; }

        public DatabasePeps()
            : base("DatabasePeps")
        {

        }

    }
}
