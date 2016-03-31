using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everglades.Models.Assets
{
    public class QuantoPut : AQuanto
    {
        public override string getName()
        {
            return "Quanto Put on asset " + underlying.getName();
        }

        public override double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public override double getPrice(DateTime t)
        {
            double S = underlying.getPrice(t);
            double Q = 1.0 / underlying.getCurrency().getChangeToEuro(t);
            Tuple<double[,], double[]> correlAndVol = AQuanto.computeCorrelationAndVol(t, underlying, underlying.getCurrency(), 200);
            double sigma1 = correlAndVol.Item2[0];
            double sigma2 = correlAndVol.Item2[1];
            double rho = correlAndVol.Item1[0, 1];
            double tau = (maturity - t).TotalDays / 365;
            wp.getPricePutQuanto(S, Q, strike, this.getCurrency().getInterestRate(t), underlying.getCurrency().getInterestRate(t),
                                    sigma1, sigma2, rho, tau);
            return wp.price;
        }

        public override Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public override string getType()
        {
            return "Quanto Put";
        }

        public override double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

    }
}
