using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AccessBD
{
    [Table("Cash")]
    public class CashDB
    {
        [Key]
        public DateTime date { get; set; }
        public double value { get; set; }
    }
}
