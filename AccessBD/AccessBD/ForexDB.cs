using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace AccessBD
{
    [Table("Forex")]
    public class ForexDB : AssetDB
    {
        public Currencies forex {get; set;}
    }
}
