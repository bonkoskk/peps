using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
            // Should access database for equity price
            return 1000;
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