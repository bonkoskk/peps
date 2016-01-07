using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Option : IDerivative
    {
        private IAsset underlying;
        public Option(IAsset underlying) {
            this.underlying = underlying;
        }
        String IAsset.Get_Name()
        {
            return "Option on " + underlying.Get_Name();
        }

        double IAsset.Get_Price()
        {
            throw new NotImplementedException();
        }

        Data IAsset.Get_Price(DateTime t1, DateTime t2, TimeSpan step)
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