using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models.Assets
{
    public class EuropeanCall : AVanillaOption
    {
        public EuropeanCall()
        {

        }

        public EuropeanCall(IAsset asset, DateTime T, double K, double sigma)
        {
            this.underlying = asset;
            this.maturity = T;
            this.strike = K;
            this.volatility = sigma;
        }
        

        public override string getType()
        {
            return "European Call";
        }

        public override string getName()
        {
            return "European Call on asset " + underlying.getName();
        }

        public override Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public override double getPrice(DateTime t)
        {
            WrapperVanilla wc = new WrapperVanilla();
            double T = (maturity - t).TotalDays / 365;
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            wc.getPriceOptionEuropeanCall(T, underlying.getPrice(), strike, underlying.getVolatility(t), getCurrency().getInterestRate(t, (maturity - t)), 0);
            return wc.getPrice();
        }

        public override double getDelta(DateTime t)
        {
            WrapperVanilla wc = new WrapperVanilla();
            double T = (maturity - t).TotalDays / 365;
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            wc.getPriceOptionEuropeanCall(T, underlying.getPrice(), strike, underlying.getVolatility(t), getCurrency().getInterestRate(t, (maturity - t)), 0);
            return wc.getDelta();
        }

        public override double getVolatility(DateTime t)
        {
            return this.underlying.getVolatility(t);
        }


        public Portfolio getDeltaPortfolio(DateTime t)
        {
            getPrice(t);
            double[] delta = new double[1]{this.getDelta(t)};

            List<IAsset> underlying_list = new List<IAsset>{underlying};
            Portfolio port = new Portfolio(underlying_list);
            int i = 0;
            foreach (IAsset ass in underlying_list)
            {
                port.addAsset(ass, delta[i]);
                i++;
            }
            return port;
        }
    }
}