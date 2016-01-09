﻿using System;
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

        //TODO
        Data IAsset.Get_Price(DateTime t1, DateTime t2, TimeSpan step)
        {
            Data data = new Data();
            DateTime t = t1;
            while (t <= t2)
            {
                double prix = 0;
                foreach (KeyValuePair<IAsset, int> pair in assetList)
                {
                    prix += pair.Value * pair.Key.Get_Price(t);
                }
                data.add(new DataPoint(t, prix));
                t += step;
            }
            return data;
        }

        //TODO
        double IAsset.Get_Price(DateTime t)
        {
            throw new NotImplementedException();
        }

        //TODO
        double IAsset.Get_Delta(DateTime t)
        {
            throw new NotImplementedException();
        }

    }
}