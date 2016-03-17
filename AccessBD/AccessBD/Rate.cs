using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessBD
{
    [Table("InterestRates")]
    public class InterestRate
    {
        [Key, Column(Order = 0)]
        public DateTime date { get; set; }
        [Key, Column(Order = 1)]
        public string currency { get; set; }
        [Key, Column(Order = 2)]
        public TimeSpan maturity { get; set; }
        public double rate { get; set; }
    }
}
