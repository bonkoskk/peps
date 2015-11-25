using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Portfolio : IAsset
    {
        public LinkedList<Tuple<IAsset, uint>> assetList;

        public Portfolio()
        {
            assetList = new LinkedList<Tuple<IAsset, uint>>();
        }

        public bool Add_Asset(IAsset a, uint number)
        {
            throw new NotImplementedException();
        }

        public bool Remove_Asset(IAsset a, uint number)
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