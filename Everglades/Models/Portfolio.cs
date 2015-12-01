﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Portfolio : IAsset
    {
        public Dictionary<IAsset, uint> assetList;

        public Portfolio()
        {
            assetList = new Dictionary<IAsset, uint>();
        }
        
        public bool Add_Asset(IAsset asset, uint number)
        {
            if (assetList.ContainsKey(asset))
            {
                assetList[asset] += number;
                return true;
            }
            else
            {
                assetList[asset] = number;
                return true;
            }
        }

        public bool Remove_Asset(IAsset a, uint number)
        {
            throw new NotImplementedException();
        }

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