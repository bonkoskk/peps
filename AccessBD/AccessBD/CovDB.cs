﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace AccessBD
{
    [Table("Covariances")]
    public class CovDB
    {
        public DateTime date { get; set; }
        public int indexX { get; set; }
        public int indexY { get; set; }

        public double value { get; set; }
    }
}
