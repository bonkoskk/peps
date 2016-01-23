using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAccessBD
{
    public class DBInitialisation
    {
        public static void DBInit(smweyoke context)
        {
            DateTime lastConn;
            DateTime beginProduct = new DateTime(2011, 3, 1);
            if (context.DbConnections.FirstOrDefault(p => p.date == context.DbConnections.Max(x => x.date)) == null)
            {
                lastConn = beginProduct;
            }
            else
            {
                lastConn = Access.GetLastConnection(context);
            }
            //List<string> list = new List<string> { "AAPL", "SAN", "0939.HK", "0941.HK", "GS", "XOM", "HSBA.L", "1398.HK", "JNJ", "MSFT", "NESN.VX", "RNO.PA", "PG", "ROG.VX", "SAN.PA", "SIE.DE", "PERH.EX", "FP.PA", "UBSG.VX", "VOD.L" };
            List<string> list = new List<string> { "AAPL", "SAN", "0939.HK", "0941.HK", "XOM", "HSBA.L", "1398.HK", "JNJ", "MSFT", "NESN.VX", "PG", "ROG.VX", "SAN.PA", "SIE.DE", "FP.PA", "UBSG.VX", "VOD.L" };
            //List<string> list = new List<string> { "AAPL", "SAN" };
            if (DateTime.Compare(lastConn, DateTime.Today) < 0 && DateTime.Today.DayOfWeek != DayOfWeek.Saturday && DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
            {
                XMLParser.XMLParser.CreateXML(list, lastConn, DateTime.Today, "C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin/YahooDataPeps.xml");
                XMLParser.XMLParser parserXML = new XMLParser.XMLParser();
                parserXML.XMLtoDB("C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin/YahooDataPeps.xml");
                if (lastConn.Equals(beginProduct))
                {
                    parserXML.list_equities.ForEach(e => context.Assets.Add(e));
                    context.SaveChanges();
                }
                parserXML.list_prices.ForEach(p => context.Prices.Add(p));
                context.SaveChanges();
                LastConnectionDB conn = new LastConnectionDB { date = DateTime.Today };
                context.DbConnections.Add(conn);
                context.SaveChanges();
            }
        }
    }
}
