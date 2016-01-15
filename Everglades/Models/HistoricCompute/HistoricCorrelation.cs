using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class HistoricCorrelation
    {
        // Based on John Hull's "Options, Futures and other derivatives" V7
        // chap.21 "Estimating volatilities and correlations"

        private static double computeCovarianceFor2(int date_nb, double[] returns1, double[] returns2)
        {
            double sum = 0;
            for (int i = 0; i < date_nb - 1; i++)
            {
                sum += returns1[i] * returns2[i];
            }
            return sum / (date_nb - 2);
        }

        internal static double[,] computeCovariance(int date_nb, int asset_nb, double[][] prices)
        {
            double[][] returns = new double[asset_nb][];
            for (int i = 0; i < asset_nb; i++)
            {
                returns[i] = ContinuouslyCompoundedReturn.compute(date_nb, prices[i]);
            }
            double[,] covariance = new double[asset_nb, asset_nb];
            for (int i = 0; i < asset_nb; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    covariance[i,j] = computeCovarianceFor2(date_nb, returns[i], returns[j]);
                    covariance[j,i] = covariance[i,j];
                }
            }
            return covariance;
        }

        internal static double[,] computeCorrelation(int date_nb, int asset_nb, double[][] prices, double[] vol)
        {
            double[][] returns = new double[asset_nb][];
            for (int i = 0; i < asset_nb; i++)
            {
                returns[i] = ContinuouslyCompoundedReturn.compute(date_nb, prices[i]);
            }
            double[,] covariance = new double[asset_nb, asset_nb];
            for (int i = 0; i < asset_nb; i++)
            {
                for (int j = 0; j <= i; j++)
                {
                    covariance[i,j] = computeCovarianceFor2(date_nb, returns[i], returns[j]) / (vol[i]*vol[j]);
                    covariance[j,i] = covariance[i,j];
                }
            }
            return covariance;
        }


    }
}