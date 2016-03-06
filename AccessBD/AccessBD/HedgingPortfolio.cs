using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessBD
{
    [Table("HedgingPortfolio")]
    public class HedgingPortfolio
    {
        [Key]
        public DateTime date { get; set; }
        public double value { get; set; }
    }
}
