using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class ComputationTools
    {
        internal static double Mean(Dictionary<DateTime, double> returns)
        {
            double S = 0;
            foreach(KeyValuePair<DateTime, double> kvp in returns)
            {
                S += kvp.Value;
            }
            return S/returns.Count();
        }

        internal static double WeightedSum(Dictionary<DateTime, double> ret1, Dictionary<DateTime, double> ret2, double lambda)
        {
            if (ret1.Count() != ret2.Count()) throw new ArgumentException("dictionnaire de taille différentes.");
            double S = 0;
            int i = 0;
            foreach(KeyValuePair<DateTime, double> kvp in ret1)
            {
                S += Math.Pow(lambda, i)*ret1[kvp.Key]*ret2[kvp.Key];
                i+=1;
            }
            return S;
        }

        internal static Dictionary<DateTime, double> Diff(Dictionary<DateTime, double> ret, double d)
        {
            Dictionary<DateTime, double> res = new Dictionary<DateTime, double>();
            foreach(KeyValuePair<DateTime, double> kvp in ret)
            {
                res[kvp.Key] = kvp.Value - d; 
            }
            return res;
        }
    }
}