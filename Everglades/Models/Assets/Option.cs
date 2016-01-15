using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models
{
    public class Option : IDerivative
    {
        private IAsset underlying;
        public Option(IAsset underlying) {
            this.underlying = underlying;
        }
        String IAsset.getName()
        {
            return "Option on " + underlying.getName();
        }

        //TODO
        double IAsset.getPrice()
        {
            // debug kevin
            // doit retourner 6.51
            WrapperQuanto wc = new WrapperQuanto();

            //wc.getPriceOptionBarrier(50000, 1, 100, 105, 0.25, 0.02, 100, 90);


            if (String.Compare(underlying.getName(), "orange") == 0)
            {
                wc.getPriceCallQuanto(100, 1.2, 100, 0.02, 0.03, 0.2, 0.2, 0.1, 1);
                //wc.getPriceOptionBarrier(50000, 1, 100, 105, 0.25, 0.02, 100, 90);
                return wc.getPrice();
            }

            return 100;

            //return wc.getPrice();
            throw new NotImplementedException();
        }

        //TODO
        Data IAsset.getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        //TODO
        double IAsset.getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        //TODO
        double IAsset.getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        string IDerivative.getType()
        {
            throw new NotImplementedException();
        }

        List<Param> IDerivative.getParam()
        {
            throw new NotImplementedException();
        }

        void IDerivative.setParam(List<Param> param)
        {
            throw new NotImplementedException();
        }


        double IAsset.getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}