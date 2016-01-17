using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAccessBD
{
    public class YahooData : System.Data.Entity.DropCreateDatabaseAlways<smweyoke>
    {
        protected override void Seed(smweyoke context)
        {
            Console.WriteLine("Dans seed");
            //List<string> list = new List<string> { "AAPL", "SAN", "0939.HK", "0941.HK", "CSGN.VX", "XOM", "HSBA.L", "1398.HK", "JNJ", "MSFT", "NESN.VX", "NOVN.VX", "PG", "ROG.VX", "SAN.PA", "SIE.DE", "TEF.TI", "FP.PA", "UBSG.VX", "VOD.L" };
            List<string> list = new List<string> { "AAPL", "SAN" };

            XMLParser.XMLParser.CreateXML(list, new DateTime(2011, 3, 1), DateTime.Now, "C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin/YahooDataPeps.xml");
            Console.WriteLine("xml créé");
            XMLParser.XMLParser parserXML = new XMLParser.XMLParser();
            parserXML.XMLtoDB("C:/Users/ensimag/Desktop/Everglades/peps/Everglades/bin/YahooDataPeps.xml");
            parserXML.list_equities.ForEach(e => context.Assets.Add(e));
            context.SaveChanges();
            parserXML.list_prices.ForEach(p => context.Prices.Add(p));
            context.SaveChanges();
        }
    }
}
