using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Wrapping;

namespace Everglades.Models
{
    public class Equity : IAsset
    {
        private string name;

        public Equity(string name)
        {
            this.name = name;
        }
        String IAsset.Get_Name()
        {
            return name;
        }
        double IAsset.Get_Price()
        {

            // debug kevin
            // doit retourner 6.51
            //WrapperBarrier wc = new WrapperBarrier();

            //wc.getPriceOptionBarrier(50000, 1, 100, 105, 0.25, 0.02, 100, 90);
            return 100;
            //return wc.getPrice();
        }

        List<double> IAsset.Get_Price(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        double IAsset.Get_Price(DateTime t)
        {
            throw new NotImplementedException();
        }

        double IAsset.Get_Delta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}