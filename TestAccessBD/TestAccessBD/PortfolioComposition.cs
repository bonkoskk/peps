using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAccessBD
{
    public class PortfolioComposition
    {
        //public int PortfolioCompositionId { get; set; }
        //clé étrangère
        [Key, Column(Order=0)]
        public int AssetId { get; set; }
        public virtual AssetDB Asset { get; set; }

        public int quantity { get; set; }
    }
}
