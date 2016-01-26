using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestAccessBD;

namespace Everglades.Models.DataBase
{
    public class AccessDB
    {
        // TODO : delete this rand
        private static RandomNormal rand = new RandomNormal();
        private static CacheDB cache = new CacheDB(1000);

        private static DateTime Round(DateTime dateTime, TimeSpan interval)
        {
            var halfIntervelTicks = (interval.Ticks + 1) >> 1;
            return dateTime.AddTicks(halfIntervelTicks - ((dateTime.Ticks + halfIntervelTicks) % interval.Ticks));
        }

        // return list of assets names
        public static List<string> Get_Asset_List()
        {
            return Access.Get_List_Assets();
        }

        // return price of an asset from name
        public static double Get_Asset_Price(String assetName, DateTime date, bool cacheUse = true)
        {
            date = Round(date, TimeSpan.FromDays(1));
            if (!cacheUse)
            {
                int id = Access.GetIdFromName(assetName);
                Dictionary<string, double> P = Access.Get_Price(id, date);
                return P["close"];
            }
            else
            {
                if (!cache.contains(assetName, date))
                {
                    int id = Access.GetIdFromName(assetName);
                    Dictionary<string, double> P = Access.Get_Price(id, date);
                    double price = P["close"];
                    cache.setPrice(assetName, date, price);
                    return price;
                }
                else
                {
                    return cache.getPrice(assetName, date);
                }
            }
        }

        // this member is temporary (TODO)
        private static Dictionary<DateTime, double> everglades_price = new Dictionary<DateTime, double>();
        public static double getEvergladesPrice(DateTime date)
        {
            date = Round(date, TimeSpan.FromDays(1));
            if (everglades_price.ContainsKey(date))
            {
                return everglades_price[date];
            }
            else
            {
                throw new NoDataException("Everglades", date);
            }
        }

        public static void setEvergladesPrice(DateTime date, double price)
        {
            date = Round(date, TimeSpan.FromDays(1));
            everglades_price[date] = price;
        }

        // return delta of an asset from name
        public static double Get_Asset_Delta(String assetName, DateTime date)
        {
            throw new NotImplementedException();
        }

        // return interest rate of a particular money
        public static double Get_Interest_Rate(String moneyName, DateTime date)
        {
            return 0.02; // TODO
        }


    }
}