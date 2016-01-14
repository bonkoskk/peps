using Everglades.Models.Assets;
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
        
        public void addAsset(IAsset asset, int number)
        {
            if (!assetList.ContainsKey(asset))
            {
                assetList[asset] = number;
            }
            else
            {
                assetList[asset] += number;
            }
        }

        public void removeAsset(IAsset asset, int number)
        {
            if (!assetList.ContainsKey(asset))
            {
                assetList[asset] = - number; ;
            }
            else
            {
                assetList[asset] -= number;
            }
        }

        public string portfolioComposition()
        {
            string str = "[";
            double equ = 0.0, vanilla = 0.0, exo = 0.0;

            foreach (KeyValuePair<IAsset, int> item in assetList)
            {
                if (item.Value != 0)
                {
                    double price = item.Value * item.Key.getPrice();
                    if (price < 0)
                    {
                        price = -price;
                    }
                    if (item.Key is Equity)
                    {
                        equ += price;
                    }
                    else if (item.Key is AVanillaOption)
                    {
                        vanilla += price;
                    }
                    else if (item.Key is IDerivative)
                    {
                        exo += price;
                    }
                }
            }

            if (assetList.Keys.Count == 0 || (equ == 0 && vanilla == 0 && exo == 0))
            {
                str += "{label: \"Empty\", data: 1}";
            }
            else
            {
                str += "{label: \"Equities\", data: " + equ + "},";
                str += "{label: \"Vanilla Options\", data: " + vanilla + "},";
                str += "{label: \"Exotic Options\", data: " + exo + "}";
            }
            str += "]";
            return str;
        }

        public String getName()
        {
            return "Portfolio of " + assetList.Count + " assets";
        }

        public double getPrice()
        {
            double price = 0;
            foreach (KeyValuePair<IAsset, int> entry in assetList)
            {
                price += entry.Value * entry.Key.getPrice();
            }
            return price;
        }

        //TODO
        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            Data data = new Data();
            DateTime t = t1;
            while (t <= t2)
            {
                double prix = 0;
                foreach (KeyValuePair<IAsset, int> pair in assetList)
                {
                    prix += pair.Value * pair.Key.getPrice(t);
                }
                data.add(new DataPoint(t, prix));
                t += step;
            }
            return data;
        }

        //TODO
        public double getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        //TODO
        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public Currency getCurrency()
        {
            throw new NotImplementedException();
        }
    }
}