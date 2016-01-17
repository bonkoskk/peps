using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public class RandomNormal : Random
    {
        /**
         * Box-Muller implementation to return
         * a normal(0,1) random variable from classic
         * uniform random variables
         * 
         **/
        public double NextNormal() {
            double u1 = NextDouble();
            double u2 = NextDouble();
            return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
        }
    }
}