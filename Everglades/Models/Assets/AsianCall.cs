using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models.Assets
{
    public class AsianCall : AVanillaOption
    {
        public override string getType()
        {
            return "Asian Call";
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
            WrapperAsian wa = new WrapperAsian();
            double T = (maturity - t).TotalDays / 365;
            int MC = (int)(T*10000);
            int J = (int)(T*250);
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            wa.getPriceCallAsian(MC, T, underlying.getPrice(t), strike, underlying.getVolatility(t), getCurrency().getInterestRate(t), J);

            return wa.getPrice();
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