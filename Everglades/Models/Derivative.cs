using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Derivative : IAsset
    {
        private IAsset underlying;

        String IAsset.Get_Name()
        {
             throw new NotImplementedException();
        }

        double IAsset.Get_Price()
        {
            throw new NotImplementedException();
        }

        double[] IAsset.Get_Price(DateTime t1, DateTime t2, TimeStep step)
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