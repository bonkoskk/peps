using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class HistoricVolatility
    {
        internal static double compute(int date_nb, double[] prices)
        {
            double sum = 0;
            double sum2 = 0;
            for (int i = 0; i < date_nb; i++)
            {
                sum += prices[i];
                sum2 += prices[i] * prices[i];
            }
            double mean = sum / date_nb;
            double var = ( sum2 - date_nb*mean*mean ) / (date_nb - 1);
            return Math.Sqrt(var);
        }
    }
}