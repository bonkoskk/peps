using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessBD{

    [Table("ForexRates")]
    public class ForexRateDB
    {
        public DateTime date { get; set; }

        public double rate { get; set; }

        public int ForexDBId { get; set; }
        public ForexDB ForexDB { get; set; }
    }
}
