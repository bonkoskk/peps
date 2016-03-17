using System;
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
        public static Dictionary<Currencies, int> _id_forex = new Dictionary<Currencies,int>(4);
        public static Dictionary<Irate, int> _id_irate = new Dictionary<Irate, int>(4);
        public static Dictionary<String, int> _id_Everglades = new Dictionary<string, int>(1);


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

        public static String GetSymbolFromName(string name)
        {
            using (var context = new qpcptfaw())
            {
                var a = from asset in context.Assets.OfType<EquityDB>()
                        where asset.name == name
                        select asset.symbol;
                if (a.Count() == 0) throw new ArgumentException("name does not exist in the database", name);
                if (a.Count() > 1) throw new ArgumentException("duplicate name in the database", name);
                return a.First();
            }
        }

        public static int GetIdEverglades()
        {
            int id = -1;
            if (_id_Everglades.ContainsKey("Everglades")) return _id_Everglades["Everglades"];
            using (var context = new qpcptfaw())
            {
                var a = from asset in context.Assets.OfType<EvergladesDB>()
                        select asset;
                if (a.Count() != 1) throw new Exception("The Everglades table is wrong : Everglades should be unique");
                id = a.First().AssetDBId;
                _id_Everglades.Add("Everglades", id);
            }
            return id;
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


        public static Currencies GetEquityCurrencyFromSymbol(string symbol)
        {
            using (var context = new qpcptfaw())
            {
                var a = from asset in context.Assets.OfType<EquityDB>()
                        where asset.symbol == symbol
                        select asset;
                if (a.Count() == 0) throw new ArgumentException("symbol does not exist in the database", symbol);
                if (a.Count() > 1) throw new ArgumentException("duplicate symbol in the database", symbol);
                return a.First().currency;
            }
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
                             select new { price = p.price, priceEur = p.priceEur })
                                .ToArray();
                if (array.Length == 0)
                {
                    array = (from p in context.Prices
                             where p.AssetDBId == id && p.date < datelocal
                             orderby p.date descending
                             select new { price = p.price, priceEur = p.priceEur })
                                .Take(1).ToArray();
                }   
                if (array.Length == 0)
                {
                    throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
                }
                else
                {
                    var price = array[0];
                    P["price"] = price.price;
                    P["priceEur"] = price.priceEur;
                    return P;
                }
            }
        }


        public static double Get_Price_Everglades(DateTime date)
        {
            DateTime datelocal = date;
            int id = -1;
            if (datelocal < DBInitialisation.DBstart)
            {
                throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
            }

            using (qpcptfaw context = new qpcptfaw())
            {
                id = GetIdEverglades();
                var array = (from p in context.Prices
                             where p.AssetDBId == id && p.date == datelocal
                             select new { price = p.price })
                                .ToArray();
                if (array.Length == 0)
                {
                    throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
                }
                else
                {
                    var price = array[0];
                    return price.price;
                }
            }
        }


        public static Price Get_PriceDB(int id, DateTime date)
        {
            DateTime datelocal = date;
            if (datelocal < DBInitialisation.DBstart)
            {
                throw new ArgumentException("no data for this date, check if this date is after the first date in the database", date.ToString());
            }

            using (qpcptfaw context = new qpcptfaw())
            {
                    var prices = from p in context.Prices
                                   where p.AssetDBId == id && p.date == date
                                   select p;
                    if (prices.Count() == 0) throw new ArgumentException("price does not exist in the database");
                    if (prices.Count() > 1) throw new ArgumentException("duplicate price data in the database");
                    return prices.First();
            }
        }


        public static Dictionary<Tuple<int, DateTime>, Dictionary<string, double>> Get_Price(List<int> ids, List<DateTime> dates)
        {
            Dictionary<Tuple<int, DateTime>, Dictionary<string, double>> dic = new Dictionary<Tuple<int, DateTime>, Dictionary<string, double>>();

            using (qpcptfaw context = new qpcptfaw())
            {
                var prices = from p in context.Prices
                             where ids.Contains(p.AssetDBId) && dates.Contains(p.date)
                             select p;
                foreach(var p in prices)
                {
                    Dictionary<string, double> P = new Dictionary<string, double>();
                    P["price"] = p.price;
                    P["priceEur"] = p.priceEur;
                    dic[new Tuple<int, DateTime>(p.AssetDBId, p.date)] = P;
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
                             select new { id = p.AssetDBId, date = p.date, price = p.price, priceEur = p.priceEur })
                                .ToArray();
                foreach (var price in array)
                {
                    Dictionary<string, double> P = new Dictionary<string, double>();
                    P["price"] = price.price;
                    P["priceEur"] = price.priceEur;
                    dic[price.date] = P;
                }
                return dic;
            }
        }

        public static double get_Price_Eur(int id, DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var prices = from p in context.Prices
                             where p.AssetDBId == id && p.date == date
                             select p;
                if (prices.Count() == 0) throw new ArgumentException("No price for this asset at this date", date.ToString());
                if (prices.Count() > 1) throw new Exception("Data should be unique");
                return prices.First().priceEur;
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

        public static DateTime GetLastData(Currencies currency)
        {
            using (var context = new qpcptfaw())
            {
                double l;
                int id = getForexIdFromCurrency(currency);
                var rates = from p in context.ForexRates
                             where p.ForexDBId == id
                             select p;
                if (rates.Count() == 0) return DBInitialisation.DBstart;
                return rates.OrderByDescending(x => x.date).First().date;
            }
        }

        public static DateTime GetLastData(Irate interestRate)
        {
            using (var context = new qpcptfaw())
            {
                //double l;
                int id = getForexRateIdFromIrate(interestRate);
                var rates = from p in context.ForexRates //est ce bien forexRate
                            where p.ForexDBId == id
                            select p;
                if (rates.Count() == 0) return DBInitialisation.DBstart;
                return rates.OrderByDescending(x => x.date).First().date;
            }
        }

        public static double GetCurrencyExchangeWithEuro(Currencies currency, DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                int limit = 5;
                for(int i=0; i<limit; i++) {
                    var rates = from p1 in context.ForexRates
                        join p2 in context.Forex on p1.ForexDBId equals p2.ForexDBId
                        where currency == p2.currency && date == p1.date
                        select new { exchange_rate = p1.rate };
                    if (rates.Count() > 0) {
                        return 1 / rates.First().exchange_rate;
                    }
                    else
                    {
                        date = date - TimeSpan.FromDays(1);
                    }
                }
                throw new ArgumentOutOfRangeException("No data for this date");
            }
        }

        public static Dictionary<Currencies, Dictionary<DateTime, double>> GetCurrenciesExchangeWithEuro(List<Currencies> currencies, List<DateTime> dates)
        {
            int number_data = currencies.Count * dates.Count;

            using (var context = new qpcptfaw())
            {
                double l;
                var rates = from p1 in context.ForexRates
                            join p2 in context.Forex on p1.ForexDBId equals p2.ForexDBId
                            where currencies.Contains(p2.currency) && dates.Contains(p1.date)
                            select new { exchange_rate = p1.rate, date = p1.date, cur = p2.currency };
                Dictionary<Currencies, Dictionary<DateTime, double>> dic = new Dictionary<Currencies, Dictionary<DateTime, double>>();
                foreach(var val in rates) {
                    if (!dic.ContainsKey(val.cur))
                    {
                        dic[val.cur] = new Dictionary<DateTime, double>();
                    }
                    dic[val.cur][val.date] = 1 / val.exchange_rate;
                }
                // on bouche les trous
                int j = 0;
                foreach (Currencies en in currencies)
                {
                    foreach (DateTime d in dates)
                    {
                        j++;
                        if (!dic[en].ContainsKey(d))
                        {
                            dic[en][d] = GetCurrencyExchangeWithEuro(en, d);
                        }
                    }
                }
                return dic;
            }
        }

        public static DateTime GetLastConnection(qpcptfaw context)
        {
                var d = context.DbConnections.FirstOrDefault(p => p.date == context.DbConnections.Max(x => x.date));
                return d.date;
        }

        /*public static List<string> getAllEquitiesSymbol(qpcptfaw context)
        {
            List<string> list_symbol = new List<string>();
            var equities = from b in context.Assets.OfType<EquityDB>()
                           select b;
            foreach (var e in equities)
            {
                list_symbol.Add(e.symbol);
            }
            return list_symbol;
        }*/

        public static bool ContainsEquitySymbol(qpcptfaw context, string symbol)
        {
            var equities = from b in context.Assets.OfType<EquityDB>()
                           where b.symbol == symbol
                           select b;
            if (equities.Count() == 0) return false;
            if (equities.Count() == 1) return true;
            throw new Exception("Data should be unique.");
        }

        /*public static List<KeyValuePair<int, DateTime>> getAllPricesKey(qpcptfaw context)
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
        }*/

        public static bool ContainsPricesKey(qpcptfaw context, int id, DateTime date)
        {
            var prices = from p in context.Prices
                         where p.AssetDBId==id && p.date==date
                         select p;
            if (prices.Count() == 0) return false;
            if (prices.Count() == 1) return true;
            throw new Exception("Data should be unique.");
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

        public static void ClearDbConnections(qpcptfaw context, int id)
        {
            var conns = from a in context.DbConnections
                        where a.LastConnectionDBId == id
                        select a;
            foreach (var a in conns)
            {
                context.DbConnections.Remove(a);
            }
            context.SaveChanges();
        }

        public static List<DateTime> getAllKeysHedgingPortfolio(qpcptfaw context)
        {
            List<DateTime> list_dates = new List<DateTime>();
            var portfolio = from p in context.Portfolio
                            select p;
            foreach (var p in portfolio) list_dates.Add(p.date);
            return list_dates;
        }

        public static HedgingPortfolio getHedgingPortfolio(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var portfolio = from p in context.Portfolio
                                where p.date == date
                                select p;
                if (portfolio.Count() == 0) throw new ArgumentException("no portfolio value for this date", date.ToString());
                return new HedgingPortfolio { date = date, value = portfolio.First().value };
            }
        }

        public static Dictionary<int, double> getHedgingPortfolioTotalComposition(DateTime date)
        {
            Dictionary<int, double> composition = new Dictionary<int, double>();
            using (var context = new qpcptfaw())
            {
                System.Linq.IQueryable<AccessBD.PortfolioComposition> comp = null;
                for (int i = 0; i < 20; i++)
                {
                    comp = from c in context.PortCompositions
                               where c.date == date
                               select c;
                    if (comp.Count() > 0)
                    {
                        break;
                    }
                    date = date - TimeSpan.FromDays(1);
                }
                if (comp == null || comp.Count() == 0) throw new ArgumentException("No data for this date", date.ToString());
                foreach (var a in comp)
                {
                    composition[a.AssetDBId] = a.quantity;
                }
                return composition;
            }
        }

        public static double getHedgingPortfolioValue(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var portfolio = from p in context.Portfolio
                                where p.date == date
                                select p;
                if (portfolio.Count() == 0) throw new ArgumentException("no portfolio value for this date", date.ToString());
                return portfolio.First().value;
            }
        }

        /*public static List<Currencies> getAllCurrencies()
        {
            using (var context = new qpcptfaw())
            {
                List<Currencies> list_res = new List<Currencies>();
                var currencies = from f in context.Forex
                                 select f;
                if (currencies.Count() == 0) throw new Exception("No currencies stored in the database");
                foreach (var c in currencies) list_res.Add(c.currency);
                return list_res;
            }
        }*/

        public static bool CurrenciesContains(Currencies c)
        {
            using (var context = new qpcptfaw())
            {
                var currencies = from f in context.Forex
                                 where f.currency == c
                                 select f;
                if (currencies.Count() == 1) return true;
                if (currencies.Count() == 0) return false;
                throw new Exception("The data should be unique. Problem in the database.");
            }
        }

        public static int getForexIdFromCurrency(Currencies c)
        {
            int id = -1;
            using (var context = new qpcptfaw())
            {
                var a = from currency in context.Forex
                        where currency.currency == c
                        select currency;
                if (a.Count() == 0) throw new ArgumentException("symbol does not exist in the database", c.ToString());
                if (a.Count() > 1) throw new ArgumentException("duplicate symbol in the database", c.ToString());
                id = a.First().ForexDBId;
                return id;
            }
        }

        /*public static List<KeyValuePair<int, DateTime>> getAllForexRateKey(qpcptfaw context)
        {
            List<KeyValuePair<int, DateTime>> list_pair = new List<KeyValuePair<int, DateTime>>();
            var rates = from r in context.ForexRates
                         select r;
            foreach (var p in rates)
            {
                list_pair.Add(new KeyValuePair<int, DateTime>(p.ForexDBId, p.date));
            }
            return list_pair;
        }*/

        public static bool ForexRateContainsKey(qpcptfaw context, DateTime date, int id)
        {
            var rates = from r in context.ForexRates
                        where r.date == date && r.ForexDBId == id
                        select r;
            if (rates.Count() == 0) return false;
            if (rates.Count() == 1) return true;
            throw new Exception("The data returned should be unique. There is a problem in the Database.");
        }

        public static double getExchangeRate(Currencies currency, DateTime date, qpcptfaw context)
        {
            int cid;
            if (_id_forex.ContainsKey(currency)) 
            { 
                cid = _id_forex[currency];
            }
            else
            {
                cid = getForexIdFromCurrency(currency);
                _id_forex.Add(currency, cid);
            }

            var rates = from r in context.ForexRates
                        where r.ForexDBId == cid && r.date == date
                        select r;
            if (rates.Count() == 0) throw new ArgumentException("No data for this date and currency", date.ToString());
            if (rates.Count() > 1) throw new Exception("The data required should be unique.");
            return rates.First().rate;
        }

        public static void Clear_Portfolio_Price(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var portfolio = from p in context.Portfolio
                                where p.date == date
                                select p;
                if (portfolio.Count() == 0) throw new ArgumentException("no portfolio value for this date", date.ToString());
                if (portfolio.Count() > 1) throw new ArgumentException("there shoud be an unique price for this date", date.ToString());
                context.Portfolio.Remove(portfolio.First());
                context.SaveChanges();
            }
        }

        public static void Clear_Everglades_Price(DateTime date){
            using(var context = new qpcptfaw()){
                int id = GetIdEverglades();
                var everg_price = from f in context.Prices
                                  where f.AssetDBId == id && f.date == date
                                  select f;
                if (everg_price.Count() == 0) throw new ArgumentException("no everglades prices for this date", date.ToString());
                if (everg_price.Count() > 1) throw new ArgumentException("there shoud be an unique price for this date", date.ToString());
                context.Prices.Remove(everg_price.First());
                context.SaveChanges();
            }
        }

        public static void Clear_Everglades_Prices()
        {
            using (var context = new qpcptfaw())
            {
                int id = GetIdEverglades();
                var everg_price = from f in context.Prices
                                  where f.AssetDBId == id 
                                  select f;
                foreach(var e in everg_price) context.Prices.Remove(e);
                context.SaveChanges();
            }
        }


        public static void Clear_Prices_After(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var prices = from f in context.Prices
                                  where f.date > date
                                  select f;
                foreach(Price p in prices ) context.Prices.Remove(p);
                context.SaveChanges();
            }

        }

        public static void Clear_Prices_After(DateTime date, int id)
        {
            using (var context = new qpcptfaw())
            {
                var prices = from f in context.Prices
                             where f.date > date && f.AssetDBId == id
                             select f;
                foreach (Price p in prices) context.Prices.Remove(p);
                context.SaveChanges();
            }

        }


        public static double[][] getCholeskyMatrix(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var mats = from m in context.CorrelVol
                           where m.date <= date && m.date >= date.AddDays(-15)
                           select m;
                return mats.OrderBy(m => m.date).Last().matrix;
            }
        }

        public static double[] getVolatilityVector(DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var mats = from m in context.CorrelVol
                           where m.date <= date && m.date >= date.AddDays(-15)
                           select m;
                return mats.OrderBy(m => m.date).Last().vol;
            }
        }

        public static bool ContainsPortCompositionsKey(qpcptfaw context, int id, DateTime date)
        {
            var prices = from p in context.PortCompositions
                         where p.AssetDBId == id && p.date == date
                         select p;
            if (prices.Count() == 0) return false;
            if (prices.Count() == 1) return true;
            throw new Exception("Data should be unique.");
        }

        public static bool ContainsHedgPortKey(qpcptfaw context, DateTime date)
        {
            var prices = from p in context.Portfolio
                         where p.date == date
                         select p;
            if (prices.Count() == 0) return false;
            if (prices.Count() == 1) return true;
            throw new Exception("Data should be unique.");
        }

        public static bool ContainsCorrelKey(qpcptfaw context, DateTime date)
        {
            var prices = from p in context.CorrelVol
                         where p.date == date
                         select p;
            if (prices.Count() == 0) return false;
            if (prices.Count() == 1) return true;
            throw new Exception("Data should be unique.");
        }

        public static double getPortfolioComposition(int AssetId, DateTime date)
        {
            using (var context = new qpcptfaw())
            {
                var comp = from c in context.PortCompositions
                           where c.AssetDBId == AssetId && c.date == date
                           select c;
                if (comp.Count() == 0) throw new ArgumentException("No data for this date", date.ToString());
                if (comp.Count() > 1) throw new Exception("Data should be unique.");
                return comp.First().quantity;
            }
        }

        public static void Clear_Portfolio_Composition(DateTime date, int assetId)
        {
            using (var context = new qpcptfaw())
            {
                var comp = from c in context.PortCompositions
                           where c.AssetDBId == assetId && c.date == date
                           select c;
                if (comp.Count() == 0) throw new ArgumentException("no data for this date", date.ToString());
                if (comp.Count() > 1) throw new ArgumentException("data should be unique for this date.", date.ToString());
                context.PortCompositions.Remove(comp.First());
                context.SaveChanges();
            }
        }

        public static Dictionary<string, Currencies> Get_Equities_Currencies()
        {
            Dictionary<string, Currencies> dic = new Dictionary<string, Currencies>();
            using (var context = new qpcptfaw())
            {
                var assets = from b in context.Assets.OfType<EquityDB>()
                             select b;
                foreach (var asset in assets)
                {
                    dic.Add(asset.name, asset.currency);
                }
            }

            return dic;
        }


        public static bool InterestRateContains(Irate ir)
        {
            using (var context = new qpcptfaw())
            {
                var interestRate = from f in context.ForexRates
                                 where f.irate == ir
                                 select f;
                if (interestRate.Count() == 1) return true;
                if (interestRate.Count() == 0) return false;
                throw new Exception("The data should be unique. Problem in the database.");
            }
        }

        public static int getForexRateIdFromIrate(Irate ir)
        {
            int id = -1;
            using (var context = new qpcptfaw())
            {
                var a = from irate in context.ForexRates
                        where irate.irate == ir
                        select irate;
                if (a.Count() == 0) throw new ArgumentException("symbol does not exist in the database", ir.ToString());
                if (a.Count() > 1) throw new ArgumentException("duplicate symbol in the database", ir.ToString());
                id = a.First().ForexDBId;
                return id;
            }
        }
    }
}
