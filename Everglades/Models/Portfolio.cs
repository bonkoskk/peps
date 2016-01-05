using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Portfolio : IAsset
    {
        public Dictionary<IAsset, int> assetList;

        public Portfolio(List<IAsset> list)
        {
            assetList = new Dictionary<IAsset, int>();
            foreach (IAsset ass in list)
            {
                assetList[ass] = 0;
            }
        }
        
        public void Add_Asset(IAsset asset, int number)
        {
            if (!assetList.ContainsKey(asset))
            {
                throw new ArgumentException("Asset not in portfolio");
            }
            else
            {
                assetList[asset] += number;
            }
        }

        public void Remove_Asset(IAsset asset, int number)
        {
            if (!assetList.ContainsKey(asset))
            {
                throw new ArgumentException("Asset not in portfolio");
            }
            else
            {
                assetList[asset] -= number;
            }
        }

        String IAsset.Get_Name()
        {
            return "Portfolio of " + assetList.Count + " assets";
        }
        double IAsset.Get_Price()
        {
            double price = 0;
            foreach (KeyValuePair<IAsset, int> entry in assetList)
            {
                price += entry.Value * entry.Key.Get_Price();
            }
            return price;
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