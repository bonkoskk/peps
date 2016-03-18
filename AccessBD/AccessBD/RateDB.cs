using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AccessBD
{
    [Table("InterestRatesTypes")]
    public class RateDB
    {
        [Key]
        public int RateDBId { get; set; }
        public Irate rate { get; set; }
        public string name { get; set; }
    }
}
