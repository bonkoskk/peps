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

        public EuropeanCall(IAsset asset, DateTime T, double K)
        {
            this.underlying = asset;
            this.maturity = T;
            this.strike = K;
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
            wc.getPriceOptionEuropeanCall(T, underlying.getPrice(t), strike, underlying.getVolatility(t), getCurrency().getInterestRate(t), 0);
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
            wc.getPriceOptionEuropeanCall(T, underlying.getPrice(t), strike, underlying.getVolatility(t), getCurrency().getInterestRate(t), 0);
            return wc.getDelta();
        }

        public override double getVolatility(DateTime t)
        {
            return this.underlying.getVolatility(t);
        }


        public Portfolio getDeltaPortfolio(DateTime t)
        {
            List<IAsset> list_asset = new List<IAsset>();
            list_asset.Add(underlying);
            Portfolio port = new Portfolio(list_asset);
            port.addAsset(underlying, this.getDelta(t));
            return port;
        }
    }
}