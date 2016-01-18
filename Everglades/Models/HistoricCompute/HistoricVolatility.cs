using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class HistoricVolatility
    {
        // Based on John Hull's "Options, Futures and other derivatives" V7
        // chap.21 "Estimating volatilities and correlations"
        internal static double compute(int date_nb, double[] prices)
        {
            double[] returns = ContinuouslyCompoundedReturn.compute(date_nb, prices);
            double sum2 = 0;
            // returns average is assumed to be zero, so we dont have to compute it
            for (int i = 0; i < date_nb - 1; i++)
            {
                sum2 += prices[i] * prices[i];
            }
            double var = sum2 / (date_nb - 2);
            return Math.Sqrt(var);
        }
    }
}