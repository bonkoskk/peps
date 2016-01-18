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
            double price = wc.getPrice();
            return wc.getPrice();
        }

        public override double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public override double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}