using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAccessBD
{
    public class Price
    {
        public DateTime date { get; set; }

        public double open { get; set; }
        public double close { get; set; }
        public double high { get; set; }
        public double low { get; set; }
        public double volume { get; set; }

        public int AssetDBId { get; set; }
        public AssetDB AssetDB { get; set; }
    }
}
