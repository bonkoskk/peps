using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Everglades.Models
{
    public class Portfolio : IAsset
    {

        public Dictionary<IAsset, double> assetList;

        public Portfolio(List<IAsset> list)
        {
            assetList = new Dictionary<IAsset, double>();
            foreach (IAsset ass in list)
            {
                assetList[ass] = 0.0;
            }
        }
        
        public void addAsset(IAsset asset, double number)
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

            foreach (KeyValuePair<IAsset, double> item in assetList)
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

            if (ModelManage.instance.cash == 0 && (assetList.Keys.Count == 0 || (equ == 0 && vanilla == 0 && exo == 0)))
            {
                str += "{label: \"Empty\", data: 1}";
            }
            else
            {
                str += "{label: \"Cash\", data: " + XmlConvert.ToString(ModelManage.instance.cash) + "},";
                str += "{label: \"Equities\", data: " + XmlConvert.ToString(equ) + "},";
                str += "{label: \"Vanilla Options\", data: " + XmlConvert.ToString(vanilla) + "},";
                str += "{label: \"Exotic Options\", data: " + XmlConvert.ToString(exo) + "}";
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
            return getPrice(DateTime.Now);
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step, double divisor)
        {
            Data data = new Data("portfolio");
            DateTime t = t1;
            while (t < t2)
            {
                data.add(new DataPoint(t, getPrice(t) / divisor));
                t += step;
            }
            data.add(new DataPoint(t2, getPrice(t2) / divisor));
            return data;
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            return getPrice(t1, t2, step, 1);
        }

        public double getPrice(DateTime t)
        {
            double price = 0;
            foreach (KeyValuePair<IAsset, double> entry in assetList)
            {
                price += entry.Value * entry.Key.getPrice(t);
            }
            return price;
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


        public double getDividend(DateTime t1, DateTime t2)
        {
            double dividend = 0;
            foreach (KeyValuePair<IAsset, double> entry in assetList)
            {
                dividend += entry.Value * entry.Key.getDividend(t1, t2);
            }
            return dividend;
        }
    }
}