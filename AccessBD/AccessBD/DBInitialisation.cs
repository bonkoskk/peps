using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public class DBInitialisation
    {
        public static DateTime DBstart = new DateTime(2010, 12, 27);

        public static void DBInit(qpcptfaw context)
        {
            DateTime lastConn;
            if (context.DbConnections.FirstOrDefault(p => p.date == context.DbConnections.Max(x => x.date)) == null)
            {
                lastConn = DBstart;
            }
            else
            {
                lastConn = Access.GetLastConnection(context);
            }

            List<Currencies> list_currencies = new List<Currencies> { Currencies.USD, Currencies.HKD, Currencies.GBP, Currencies.CHF };
            List<string> list = new List<string> { "AAPL:US", "SAN:SM", "939:HK", "941:HK", "CSGN:VX", "XOM:US", "HSBA:LN", "1398:HK", "JNJ:US", "MSFT:US", "NESN:VX", "NOVN:VX", "PG", "ROG:VX", "SAN:FP", "SIE:GR", "TEF:SM", "FP:FP", "UBSG:VX", "VOD:LN" };
        
            //récupération des données Quandl
            if (DateTime.Compare(lastConn, DateTime.Today) < 0)
            {
                DateTime begin = lastConn;
                DateTime end = lastConn.AddYears(1);
                while (DateTime.Compare(end, DateTime.Today) < 0)
                {
                    //récupération des taux de change
                    QuandlDataExchange.storeAllInDB(list_currencies, context, begin, end);
                    //récupération des prix des actions
                    QuandlData.storeAllInDB(list, context, begin, end);
                    begin = end;
                    end = begin.AddYears(1);
                    LastConnectionDB conn = new LastConnectionDB { date = begin };
                    context.DbConnections.Add(conn);
                    context.SaveChanges();
                }
                //récupération des taux de change
                QuandlDataExchange.storeAllInDB(list_currencies, context, begin, DateTime.Today);
                QuandlData.storeAllInDB(list, context, begin, DateTime.Today);
                LastConnectionDB connection = new LastConnectionDB { date = DateTime.Today };
                context.DbConnections.Add(connection);
                context.SaveChanges();
            }

            //ajout de Everglades
            EvergladesData.addEverglades();

            /*DateTime end = new DateTime(2010, 12, 31);
            XMLParser.XMLParser.CreateXML(list, DBstart, end, XMLParser.XMLParser.dir+"/YahooDataPeps.xml");
            XMLParser.XMLParser parserXML = new XMLParser.XMLParser();
            parserXML.XMLtoDB(XMLParser.XMLParser.dir + "/YahooDataPeps.xml", context);*/

            //QuandlData.storeAllInDB(new List<string> { "FP:FP" }, context, DBInitialisation.DBstart, DateTime.Today);

        }
    }
}


