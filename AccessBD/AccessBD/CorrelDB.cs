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
    [Table("CorrelMat")]
    public class CorrelDB
    {
        public DateTime date { get; set; }
        public int indexX { get; set; }
        public int indexY { get; set; }

        public double value { get; set; }
    }
}
