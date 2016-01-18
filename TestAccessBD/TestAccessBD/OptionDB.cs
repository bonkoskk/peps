using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAccessBD
{
    [Table("Options")]
    public class OptionDB : AssetDB
    {
        public int underlyingId { get; set; }
        public EquityDB underlying { get; set; }

        public DateTime maturity { get; set; }
        public double strike { get; set; }

    }
}
