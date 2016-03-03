﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.DataProvider.PostgreSQL;


namespace AccessBD
{
    public class Access
    {
        public static List<KeyValuePair<String, int>> _id_name = new List<KeyValuePair<String, int>>();
        public static List<String> _name = new List<string>();


        /*public void AccessData()
        {
            using (var context = new smweyoke())
            {
                var a = from b in context.Assets
                        select b;
                foreach(var p in a)
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine(p.AssetDBId);
                    Console.WriteLine(p.name);
                    
                }

                var c = from b in context.Prices
                        select b;
                foreach (var p in c)
                {
                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine(p.AssetDBId);
                    Console.WriteLine(p.date);
                    Console.WriteLine(p.close);
                }
            }
        }*/
        public static List<string> Get_List_Assets()
        {
            List<string> list_assets = new List<string>();
            using (var context = new qpcptfaw())
            {
                var assets = from b in context.Assets.OfType<EquityDB>()
                             select b;
                foreach (var asset in assets)
                {
                    list_assets.Add(asset.name);
                }
            }

            return list_assets;
        }

        public static int GetIdFromName(string name)
        {
            int id = -1;
            if (_name.Count()==0){
                using (var context = new qpcptfaw())
                {
                    var a = from asset in context.Assets.OfType<EquityDB>()
                            where asset.name == name
                            select asset;
                    if (a.Count() == 0) throw new ArgumentException("name does not exist in the database", name);
                    if (a.Count() > 1) throw new ArgumentException("duplicate name in the database", name);
                    id = a.First().AssetDBId;
                    _id_name.Add(new KeyValuePair<string,int>(name, id));
                    _name.Add(name);
                }
                return id;
            }else{
                if (_name.Contains(name))
                {
                    KeyValuePair<String, int> pair = _id_name.Find(p => p.Key == name);
                    return pair.Value;
                }
                else
                {
                    using (var context = new qpcptfaw())
                    {
                        var a = from asset in context.Assets.OfType<EquityDB>()
                                where asset.name == name
                                select asset;
                        if (a.Count() == 0) throw new ArgumentException("name does not exist in the database", name);
                        if (a.Count() > 1) throw new ArgumentException("duplicate name in the database", name);
                        id = a.First().AssetDBId;
                        _id_name.Add(new KeyValuePair<string, int>(name, id));
                        _name.Add(name);
                    }
                    return id;
                }
            }
        }

        public static int GetEquityIdFromSymbol(string symbol)
        {
            int id = -1;
            using (var context = new qpcptfaw())
            {
                var a = from asset in context.Assets.OfType<EquityDB>()
                        where asset.symbol == symbol
                        select asset;
                if (a.Count() == 0) throw new ArgumentException("symbol does not exist in the database", symbol);
                if (a.Count() > 1) throw new ArgumentException("duplicate symbol in the database", symbol);
                id = a.First().AssetDBId;
            }
            return id;
        }

        public static Dictionary<string, double> Get_Price(int id, DateTime date)
        {
            DateTime datelocal = date;
            if (datelocal < DBInitialisation.DBstart)
            {
                throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
            }
            Dictionary<string, double> P = new Dictionary<string, double>();

            using (qpcptfaw context = new qpcptfaw())
            {
                var array = (from p in context.Prices
                            where p.AssetDBId == id && p.date == datelocal
                             select new { high = p.high, low = p.low, close = p.close, open = p.open, volume = p.volume })
                                .ToArray();
                if (array.Length == 0)
                {
                    array = (from p in context.Prices
                             where p.AssetDBId == id && p.date < datelocal
                             orderby p.date descending
                             select new { high = p.high, low = p.low, close = p.close, open = p.open, volume = p.volume })
                                .Take(1).ToArray();
                }   
                if (array.Length == 0)
                {
                    throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
                }
                else
                {
                    var price = array[0];
                    P["high"] = price.high;
                    P["low"] = price.low;
                    P["close"] = price.close;
                    P["open"] = price.open;
                    P["volume"] = price.volume;
                    return P;
                }
            }
        }

        public static Dictionary<Tuple<int, DateTime>, Dictionary<string, double>> Get_Price(List<int> ids, List<DateTime> dates)
        {
            Dictionary<Tuple<int, DateTime>, Dictionary<string, double>> dic = new Dictionary<Tuple<int, DateTime>, Dictionary<string, double>>();

            using (qpcptfaw context = new qpcptfaw())
            {
                var array = (from p in context.Prices
                             where ids.Contains(p.AssetDBId) && dates.Contains(p.date)
                             select new { id = p.AssetDBId, date = p.date, high = p.high, low = p.low, close = p.close, open = p.open, volume = p.volume })
                                .ToArray();
                foreach(var price in array)
                {
                    Dictionary<string, double> P = new Dictionary<string, double>();
                    P["high"] = price.high;
                    P["low"] = price.low;
                    P["close"] = price.close;
                    P["open"] = price.open;
                    P["volume"] = price.volume;
                    dic[new Tuple<int, DateTime>(price.id, price.date)] = P;
                }
                return dic;
            }
        }

        public static Dictionary<DateTime, Dictionary<string, double>> Get_Price(int id, List<DateTime> dates)
        {
            Dictionary<DateTime, Dictionary<string, double>> dic = new Dictionary<DateTime, Dictionary<string, double>>();

            using (qpcptfaw context = new qpcptfaw())
            {
                var array = (from p in context.Prices
                             where id == p.AssetDBId && dates.Contains(p.date)
                             select new { id = p.AssetDBId, date = p.date, high = p.high, low = p.low, close = p.close, open = p.open, volume = p.volume })
                                .ToArray();
                foreach (var price in array)
                {
                    Dictionary<string, double> P = new Dictionary<string, double>();
                    P["high"] = price.high;
                    P["low"] = price.low;
                    P["close"] = price.close;
                    P["open"] = price.open;
                    P["volume"] = price.volume;
                    dic[price.date] = P;
                }
                return dic;
            }
        }

        public static EquityDB GetEquityFromSymbol(string symbol)
        {
            using(var context = new qpcptfaw())
            {
                var equities = from e in context.Assets.OfType<EquityDB>()
                               where e.symbol == symbol
                               select e;
                if (equities.Count() == 0) throw new ArgumentException("symbol does not exist in the database", symbol);
                if (equities.Count() > 1) throw new ArgumentException("duplicate symbol in the database", symbol);
                return equities.First();
            }
        }

        public static DateTime GetLastData(string symbol)
        {
            using (var context = new qpcptfaw())
            {
                double l;
                int id = GetEquityIdFromSymbol(symbol);
                var prices = from p in context.Prices
                             where p.AssetDBId == id
                             select p;
                if (id == 62) 
                    l = 1;
                if (prices.Count() == 0) return DBInitialisation.DBstart;
                return prices.OrderByDescending(x => x.date).First().date;
            }
        }

        public static DateTime GetLastConnection(qpcptfaw context)
        {
                var d = context.DbConnections.FirstOrDefault(p => p.date == context.DbConnections.Max(x => x.date));
                return d.date;
        }

        public static List<string> getAllEquitiesSymbol(qpcptfaw context)
        {
            List<string> list_symbol = new List<string>();
            var equities = from b in context.Assets.OfType<EquityDB>()
                           select b;
            foreach (var e in equities)
            {
                list_symbol.Add(e.symbol);
            }
            return list_symbol;
        }

        public static List<KeyValuePair<int, DateTime>> getAllPricesKey(qpcptfaw context)
        {
            List<KeyValuePair<int, DateTime>> list_pair = new List<KeyValuePair<int, DateTime>>();
            var prices = from p in context.Prices
                         select p;
            int test = 0;
            foreach (var p in prices)
            {
                test++;
                list_pair.Add(new KeyValuePair<int, DateTime>(p.AssetDBId, p.date));
            }
            return list_pair;
        }

        public static void ClearAssets(qpcptfaw context)
        {
            var assets = from a in context.Assets
                         select a;
            foreach (var a in assets)
            {
                context.Assets.Remove(a);
            }
            context.SaveChanges();
        }

        public static void ClearAsset(qpcptfaw context, int id)
        {
            var assets = from a in context.Assets
                         where a.AssetDBId == id
                         select a;
            foreach (var a in assets)
            {
                context.Assets.Remove(a);
            }
            context.SaveChanges();
        }

        public static void ClearPrices(qpcptfaw context)
        {
            var assets = from a in context.Prices
                         select a;
            foreach (var a in assets)
            {
                context.Prices.Remove(a);
            }
            context.SaveChanges();
        }

        public static void ClearPrice(qpcptfaw context, int id)
        {
            var assets = from a in context.Prices
                         where a.AssetDBId==id
                         select a;
            foreach (var a in assets)
            {
                context.Prices.Remove(a);
            }
            context.SaveChanges();
        }

        public static void ClearDbConnections(qpcptfaw context)
        {
            var conns = from a in context.DbConnections
                         select a;
            foreach (var a in conns)
            {
                context.DbConnections.Remove(a);
            }
            context.SaveChanges();
        }

    }
}