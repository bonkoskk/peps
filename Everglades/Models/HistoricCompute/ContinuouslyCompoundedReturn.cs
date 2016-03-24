using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class ContinuouslyCompoundedReturn
    {
        // Based on John Hull's "Options, Futures and other derivatives" V7
        // chap.21 "Estimating volatilities and correlations"
        internal static double[] compute(int date_nb, double[] prices)
        {
            double[] returns = new double[date_nb - 1];
            for (int i = 0; i < date_nb - 1; i++)
            {
                returns[i] = (prices[i + 1] - prices[i]) / prices[i]; 
            }
            return returns;
        }
        
    }
}