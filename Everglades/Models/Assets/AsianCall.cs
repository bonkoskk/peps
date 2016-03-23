using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models.Assets
{
    public class AsianCall : AVanillaOption
    {
        public static int N = 10;
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
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            //wa.getPriceCallAsian(5000,)
            wa.getPriceCallAsian(5000, 2, underlying.getPrice(t), strike, 0.3, 0.05, AsianCall.N);

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