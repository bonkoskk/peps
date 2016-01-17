using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TestAccessBD
{
    public class PortfolioValue
    {
        [Key]
        public DateTime date { get; set; }
        public double price { get; set; }
    }
}
