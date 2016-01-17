using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace TestAccessBD
{
    [Table("Equities")]
    public class EquityDB : AssetDB
    {
        public string symbol { get; set; }
    }
}
