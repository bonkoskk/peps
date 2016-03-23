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
        public double open { get; set;}
        public double openEur { get; set; }
        public double high { get; set; }
        public double highEur { get; set; }
        public double low { get; set; }
        public double lowEur { get; set; }
        public double volume { get; set; }
        
        public int AssetDBId { get; set; }
        public AssetDB AssetDB { get; set; }
    }
}
