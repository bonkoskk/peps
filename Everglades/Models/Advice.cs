using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Advice
    {
        public double sensibility;
        public string relative_to;
        public string advice;

        public Advice(double sensibility, string relative_to, string advice)
        {
            this.sensibility = sensibility;
            this.relative_to = relative_to;
            this.advice = advice;
        }
    }
}