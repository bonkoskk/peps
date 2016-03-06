using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace AccessBD
{
    public enum Currencies
    {
        USD,
        EUR,
        CHF
    }

    [Table("Forex")]
    public class ForexDB
    {
        [Key]
        public int ForexDBId { get; set;}
        public Currencies from;
        public Currencies to;
    }
}
