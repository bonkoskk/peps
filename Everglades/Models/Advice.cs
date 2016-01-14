using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Advice
    {
        public double product_sensibility;
        public double hedge_sensibility;
        public string relative_to;
        public string advice;

        public Advice(double product_sensibility, double hedge_sensibility, string relative_to, string advice)
        {
            this.product_sensibility = product_sensibility;
            this.hedge_sensibility = hedge_sensibility;
            this.relative_to = relative_to;
            this.advice = advice;
        }
    }
}