using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models.Assets
{
    public class AmericanPut : AVanillaOption
    {
        public static int N = 10;
        public override string getType()
        {
            return "American Put";
        }

        public override string getName()
        {
            throw new NotImplementedException();
        }

        public override Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public override double getPrice(DateTime t)
        {
            WrapperAmerican wc = new WrapperAmerican();
            double T = (maturity - t).TotalDays / 365;
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            wc.getPricePutAmerican(underlying.getPrice(t), strike, T, getCurrency().getInterestRate(t), underlying.getVolatility(t), AmericanPut.N);
            return wc.getPrice();
        }

        public override double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public override double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}