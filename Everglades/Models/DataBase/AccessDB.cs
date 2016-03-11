using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AccessBD;

namespace Everglades.Models.DataBase
{
    public class AccessDB
    {
        // TODO : delete this rand
        private static RandomNormal rand = new RandomNormal();
        private static CacheDB cache = new CacheDB(1000);

        private static DateTime Round(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        // return list of assets names
        public static List<string> Get_Asset_List()
        {
            return Access.Get_List_Assets();
        }

        // return price of an asset from name
        public static double Get_Asset_Price(String assetName, DateTime date, bool cacheUse = true)
        {
            date = Round(date);
            if (!cacheUse)
            {
                int id = Access.GetIdFromName(assetName);
                Dictionary<string, double> P = Access.Get_Price(id, date);
                return P["price"];
            }
            else
            {
                if (!cache.contains(assetName, date))
                {
                    int id = Access.GetIdFromName(assetName);
                    Dictionary<string, double> P = Access.Get_Price(id, date);
                    double price = P["price"];
                    cache.setPrice(assetName, date, price);
                    return price;
                }
                else
                {
                    return cache.getPrice(assetName, date);
                }
            }
        }

        public static Dictionary<Tuple<String, DateTime>, double> Get_Asset_Price(List<String> assetNames, List<DateTime> dates)
        {
            Dictionary<int, string> idToName = new Dictionary<int, string>();
            foreach(string name in assetNames) {
                idToName[Access.GetIdFromName(name)] = name;
            }

            Dictionary<Tuple<int, DateTime>, Dictionary<string, double>> pricesId = Access.Get_Price(idToName.Keys.ToList(), dates);
            Dictionary<Tuple<String, DateTime>, double> pricesName = new Dictionary<Tuple<string,DateTime>,double>();
            foreach (int id_asset in idToName.Keys)
            {
                foreach (DateTime date in dates)
                {
                    Tuple<String, DateTime> str_date = new Tuple<String, DateTime>(idToName[id_asset], date);
                    Tuple<int, DateTime> id_date = new Tuple<int, DateTime>(id_asset, date);
                    if (pricesId.ContainsKey(id_date))
                    {
                        pricesName[str_date] = pricesId[id_date]["price"];
                    }
                    else
                    {
                        pricesName[str_date] = Access.Get_Price(id_asset, date)["price"];
                    }
                }
            }
            return pricesName;
        }

        public static Dictionary<DateTime, double> Get_Asset_Price(String name, List<DateTime> dates)
        {
            int id = Access.GetIdFromName(name);

            Dictionary<DateTime, Dictionary<string, double>> pricesId = Access.Get_Price(id, dates);
            Dictionary<DateTime, double> pricesName = new Dictionary<DateTime, double>();
            foreach (DateTime date in dates)
            {
                if (pricesId.ContainsKey(date))
                {
                    pricesName[date] = pricesId[date]["price"];
                }
                else
                {
                    pricesName[date] = Access.Get_Price(id, date)["price"];
                }
            }
            return pricesName;
        }

        //retourne prix d'une action en euro
        public static double Get_Asset_Price_Eur(string name, DateTime date){
            int id = Access.GetIdFromName(name);
            return Access.get_Price_Eur(id, date);
        }

        public static double getEvergladesPrice(DateTime date)
        {
            date = Round(date);
            double price;
            try {
                price = Access.Get_Price_Everglades(date);
                return price;
            } catch(ArgumentException) {
                throw new NoDataException("Exception", date);
            }
        }

        public static void setEvergladesPrice(DateTime date, double price)
        {
            date = Round(date);
            AccessBD.Write.storeEvergladesPrice(date, price);
        }


        public static double getPortfolioValue(DateTime date)
        {
            //date = Round(date, TimeSpan.FromDays(1));
            double value;
            try
            {
                value = Access.getHedgingPortfolioValue(date);
                return value;
            }
            catch (ArgumentException)
            {
                throw new NoDataException("Exception", date);
            }
        }

        public static void setHedgingPortfolioValue(DateTime date, double value)
        {
            date = Round(date);
            AccessBD.Write.storePortfolioValue(date, value);
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