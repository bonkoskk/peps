using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;


namespace Everglades.Models.Assets
{
    public class AmericanCall : AVanillaOption
    {
        public override string getType()
        {
            return "American Call";
        }

        public override string getName()
        {
            return "American Call on asset " + underlying.getName();
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

        public override double getVolatility(DateTime t)
        {
            return this.underlying.getVolatility(t);
        }

        public override double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

    }
}


        



