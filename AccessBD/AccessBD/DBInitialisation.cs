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

            List<string> list = new List<string> { "AAPL:US", "SAN:SM", "939:HK", "941:HK", "CSGN:VX", "XOM:US", "HSBA:LN", "1398:HK", "JNJ:US", "MSFT:US", "NESN:VX", "NOVN:VX", "PG", "ROG:VX", "SAN:FP", "SIE:GR", "TEF:SM", "FP:FP", "UBSG:VX", "VOD:LN" };
            



            /*if (DateTime.Compare(lastConn, DateTime.Today) < 0)
            {
                DateTime begin = lastConn;
                DateTime end = lastConn.AddMonths(1);
                XMLParser.XMLParser parserXML = new XMLParser.XMLParser();
                while(DateTime.Compare(end, DateTime.Today)<0){
                    XMLParser.XMLParser.CreateXML(list, begin, end, XMLParser.XMLParser.dir + "YahooDataPeps.xml");
                    parserXML.XMLtoDB(XMLParser.XMLParser.dir + "YahooDataPeps.xml", context);
                    begin = end;
                    end = begin.AddMonths(1);
                    LastConnectionDB conn = new LastConnectionDB { date = begin };
                    context.DbConnections.Add(conn);
                    context.SaveChanges();
                }
                XMLParser.XMLParser.CreateXML(list, begin, DateTime.Today, XMLParser.XMLParser.dir + "YahooDataPeps.xml");
                parserXML.XMLtoDB(XMLParser.XMLParser.dir + "YahooDataPeps.xml", context);
                LastConnectionDB connection = new LastConnectionDB { date = DateTime.Today };
                context.DbConnections.Add(connection);
                context.SaveChanges();           
            }*/




            if (DateTime.Compare(lastConn, DateTime.Today) < 0)
            {
                DateTime begin = lastConn;
                //DateTime end = lastConn.AddMonths(1);
                DateTime end = lastConn.AddYears(1);
                while (DateTime.Compare(end, DateTime.Today) < 0)
                {
                    QuandlData.storeAllInDB(list, context, begin, end);
                    begin = end;
                    //end = begin.AddMonths(1);
                    end = begin.AddYears(1);
                    LastConnectionDB conn = new LastConnectionDB { date = begin };
                    context.DbConnections.Add(conn);
                    context.SaveChanges();
                }
                QuandlData.storeAllInDB(list, context, begin, DateTime.Today);
                LastConnectionDB connection = new LastConnectionDB { date = DateTime.Today };
                context.DbConnections.Add(connection);
                context.SaveChanges();
            }


            /*DateTime end = new DateTime(2010, 12, 31);
            XMLParser.XMLParser.CreateXML(list, DBstart, end, XMLParser.XMLParser.dir+"/YahooDataPeps.xml");
            XMLParser.XMLParser parserXML = new XMLParser.XMLParser();
            parserXML.XMLtoDB(XMLParser.XMLParser.dir + "/YahooDataPeps.xml", context);*/

            //QuandlData.storeAllInDB(new List<string> { "FP:FP" }, context, DBInitialisation.DBstart, DateTime.Today);

        }
    }
}


