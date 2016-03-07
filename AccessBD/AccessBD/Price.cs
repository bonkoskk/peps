using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessBD
{
    public class Price
    {
        public DateTime date { get; set; }

        public double price { get; set; }
        public double priceEur { get; set; }

        public int AssetDBId { get; set; }
        public AssetDB AssetDB { get; set; }
    }
}
