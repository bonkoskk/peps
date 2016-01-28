using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using APIFiMag;
using APIFiMag.Datas;
using APIFiMag.Exporter;
using APIFiMag.Importer;
using System.Xml.Linq;
using System.Xml;

namespace AccessBD.XMLParser
{
    public class XMLParser
    {
        public static readonly string dir = System.AppDomain.CurrentDomain.BaseDirectory;

        public static void CreateXML(List<string> list_equities, DateTime beginning, DateTime end, string xmlfilePath)
        {
            List<HistoricalColumn> columns = new List<HistoricalColumn>();
            columns.Add(HistoricalColumn.Close);
            columns.Add(HistoricalColumn.High);
            columns.Add(HistoricalColumn.Open);
            columns.Add(HistoricalColumn.Volume);
            columns.Add(HistoricalColumn.Low);
            DataActif donnees = new DataActif(list_equities, columns, beginning, end);
            Environment.CurrentDirectory = dir;
            donnees.ImportData(new ImportYahoo());
            donnees.Export(new ExportXML(xmlfilePath));
        }

        public string GetNameFromSymbol(string symbol)
        {
            switch (symbol)
            {
                case "AAPL":
                    return "Apple Inc.";
                case "SAN":
                    return "Banco Santander, S.A.";
                case "0939.HK":
                    return "China Construction Bank Corporation";
                case "0941.HK":
                    return "China Mobile Limited";
                case "CSGN.VX":
                    return "Credit Suisse Group AG";
                case "XOM":
                    return "Exxon Mobil Corporation";
                case "HSBA.L":
                    return "HSBC Holdings plc";
                case "1398.HK":
                    return "Industrial and Commercial Bank of China Limited";
                case "JNJ":
                    return "Johnson & Johnson";
                case "MSFT":
                    return "Microsoft Corporation";
                case "NESN.VX":
                    return "NESTLE N";
                case "NOVN.VX":
                    return "NOVARTIS N";
                case "PG":
                    return "The Procter & Gamble Company";
                case "ROG.VX":
                    return "Roche Holding AG";
                case "SAN.PA":
                    return "Sanofi";
                case "SIE.DE":
                    return "Siemens Aktiengesellschaft";
                case "TEF.TI":
                    return "TELEFONICA";
                case "FP.PA":
                    return "TOTAL S.A.";
                case "UBSG.VX":
                    return "UBS GROUP N";
                case "VOD.L":
                    return "Vodafone Group Plc";
                case "GS":
                    return "The Goldman Sachs Group, Inc.";
                case "RNO.PA":
                    return "Renault SA";
                case "PERH.EX":
                    return "PERNOD RICARD";
                default:
                    return "";
            }

        }

        public void XMLtoDB(string xmlfilePath, qpcptfaw context)
        {
            XElement xelement = XElement.Load(xmlfilePath);
            IEnumerable<XElement> data = xelement.Elements();
            string symbol;
            string column;
            double value;
            DateTime date;

            List<Price> list_prices = new List<Price>();

            List<string> list_symbols = new List<string>();
            List<KeyValuePair<int, DateTime>> list_pair = new List<KeyValuePair<int, DateTime>>(); 
            int aid;
            Price price;

            KeyValuePair<int, DateTime> keyValue;

            List<string> list_symbols_db = Access.getAllEquitiesSymbol(context);
            List<KeyValuePair<int, DateTime>> list_pair_db = Access.getAllPricesKey(context);

            foreach (var d in data)
            {
                symbol = d.Element("{http://tempuri.org/DataSet.xsd}Symbol").Value;
                column = d.Element("{http://tempuri.org/DataSet.xsd}Column").Value;
                value = XmlConvert.ToDouble(d.Element("{http://tempuri.org/DataSet.xsd}Value").Value);
                date = DateTime.Parse(d.Element("{http://tempuri.org/DataSet.xsd}Date").Value);

                if (!list_symbols_db.Contains(symbol))
                {
                    EquityDB e = new EquityDB { name = GetNameFromSymbol(symbol), symbol = symbol };
                    context.Assets.Add(e);
                    context.SaveChanges();
                    AssetDB.assetCounter();
                    list_symbols_db = Access.getAllEquitiesSymbol(context);
                }


                //Console.Write("Liste equities : ");
                //Console.WriteLine(this.list_equities);
                aid = Access.GetEquityIdFromSymbol(symbol);
                if (aid != -1)
                {
                    keyValue = new KeyValuePair<int, DateTime>(aid, date);
                    if (!list_pair_db.Contains(keyValue))
                    {
                        if (!list_pair.Contains(keyValue))
                        {
                            Price p = new Price { AssetDBId = aid, date = date };
                            switch (column)
                            {
                                case "Close":
                                    p.close = value;
                                    break;
                                case "Open":
                                    p.open = value;
                                    break;
                                case "High":
                                    p.high = value;
                                    break;
                                case "Low":
                                    p.low = value;
                                    break;
                                case "Volume":
                                    p.volume = value;
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }
                            list_prices.Add(p);
                            list_pair.Add(new KeyValuePair<int, DateTime>(p.AssetDBId, date));
                        }
                        else
                        {
                            price = list_prices.Find(x => (x.AssetDBId == aid && x.date == date));
                            switch (column)
                            {
                                case "Close":
                                    price.close = value;
                                    break;
                                case "Open":
                                    price.open = value;
                                    break;
                                case "High":
                                    price.high = value;
                                    break;
                                case "Low":
                                    price.low = value;
                                    break;
                                case "Volume":
                                    price.volume = value;
                                    break;
                                default:
                                    Console.WriteLine("Default case");
                                    break;
                            }
                        }
                    }

                }
            }
            list_prices.ForEach(p => context.Prices.Add(p));
            context.SaveChanges();
            //Console.Write("Liste prices : ");
            //this.list_prices.ForEach(p => { Console.Write(p.AssetDBId); Console.Write(" "); Console.Write(p.date); Console.Write(" "); Console.Write(p.close); Console.Write(" "); Console.Write(p.high); Console.Write(" "); Console.Write(p.low); Console.Write(" "); Console.Write(p.open); Console.Write(" "); Console.WriteLine(p.volume); });
        }
    }
}
