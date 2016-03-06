using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccessBD
{
    [Table("Portfolio Composition")]
    public class PortfolioComposition
    {
        public DateTime date { get; set; }

        public int AssetDBId { get; set; }
        public AssetDB AssetDB { get; set; }

        public double quantity { get; set; }
    }
}
