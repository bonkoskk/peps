using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToDB.DataProvider.PostgreSQL;


namespace TestAccessBD
{
    public class Access
    {
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
            using (var context = new smweyoke()){
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
            using (var context = new smweyoke())
            {
                var a = from asset in context.Assets.OfType<EquityDB>()
                        select asset;
                foreach (var asset in a)
                {
                    if (asset.name == name)
                    {
                        id = asset.AssetDBId;
                        break;
                    }
                }

            }
            return id;
        }

        public static Dictionary<string, double> Get_Price(int id, DateTime date)
        {
            DateTime datelocal = date;
            Dictionary<string, double> P = new Dictionary<string, double>();
            using (var context = new smweyoke())
            {
                var price = context.Prices.Find(id, datelocal);
                while (price == null) 
                {
                    datelocal = datelocal.AddDays(-1);
                    price = context.Prices.Find(id, datelocal);
                }
                P["high"] = price.high;
                P["low"] = price.low;
                P["close"] = price.close;
                P["open"] = price.open;
                P["volume"] = price.volume;
            }
            return P;
        }

        public static DateTime GetLastConnection(smweyoke context)
        {
                var d = context.DbConnections.FirstOrDefault(p => p.date == context.DbConnections.Max(x => x.date));
                return d.date;
        }
        
    }
}
