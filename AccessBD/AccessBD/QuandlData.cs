using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuandlCS.Requests;
using QuandlCS.Types;
using QuandlCS.Connection;
using QuandlCS.Interfaces;
using Newtonsoft.Json.Linq;

namespace AccessBD
{
    class QuandlData
    {
        //YAHOO/AAPL
        public static string GetJsonData(string source, string codeQuandl, DateTime start, DateTime end){
            QuandlDownloadRequest request = new QuandlDownloadRequest();
            //request.APIKey = " _qjPeu6SzNYNJ42scYAi";
            request.APIKey = "1ztyfXyfPBTeqfshhBdE";

            if (codeQuandl == "EPA_SAN")
            {
                if (end.Year < 2014)
                {
                    request.Datacode = new Datacode("YAHOO", "PA_SAN");
                }
                else
                {
                    request.Datacode = new Datacode(source, codeQuandl);
                }
            }
            else
            {
                request.Datacode = new Datacode(source, codeQuandl);
            }
            request.Format = FileFormats.JSON;
            request.Frequency = Frequencies.Daily;
            request.Transformation = Transformations.None;
            request.StartDate = start;
            request.EndDate = end;

            //Console.WriteLine("The request string is : {0}", request.ToRequestString());
            IQuandlConnection connection = new QuandlConnection();
            return connection.Request(request);
            //JObject jObject = JObject.Parse(data);
            //JArray column_names = (JArray)jObject["column_names"];
        }

        public static void storeInDB(string json, qpcptfaw context, string source, string code)
        {
            JObject jObj = JObject.Parse(json);
            JArray datas = (JArray)jObj["data"];

            //tous les symboles présents dans la BD
            List<string> list_symbols_db = Access.getAllEquitiesSymbol(context);
            //récupère le symbole de l'action récupérée dans ce json
            //string symbol = (string)jObj["code"];
            string symbol = code;

            int aid;
            Currencies curr;

            //récupère tous les clés id-date de la BD (table Prices)
            List<KeyValuePair<int, DateTime>> list_pair_db = Access.getAllPricesKey(context);

            KeyValuePair<int, DateTime> keyValue;
            List<Price> list_prices = new List<Price>();

            double c = 0;
            string name;

            //si la bd ne contient pas le symbole concerné, on crée une nouvelle action et on la stocke dans la BD (table asset)
            if (!list_symbols_db.Contains(symbol))
            {
                name = (string)jObj["name"];
                EquityDB e = new EquityDB { name = name, symbol = symbol, currency = CurrencyAsset.getCurrencyOf(name) };
                context.Assets.Add(e);
                context.SaveChanges();
                AssetDB.assetCounter();
            }

            //["Date","Open","High","Low","Close","Volume","Adjusted Close"]

            //récupère l'id correspondant au symbole de l'action
            aid = Access.GetEquityIdFromSymbol(symbol);
            curr = Access.GetEquityCurrencyFromSymbol(symbol);
            //si l'id est -1 alors le symbol n'existe pas dans la bd, on ne stocke pas le prix
            if(aid!=-1){
                //on parse les données json
                 foreach (var item in datas.Children())
                {
                     //chaque item correspond aux données d'un jour
                    JToken[] data = item.ToArray();
                    DateTime date = DateTime.Parse(data[0].ToString());
                    keyValue = new KeyValuePair<int, DateTime>(aid, date);
                     //si la bd ne contient pas les données pour cette action pour ce jour, on ajoute les données  
                    if (!list_pair_db.Contains(keyValue)){
                        if (source == "YAHOO")
                        {
                            c = double.Parse(data[6].ToString());
                        }
                        else if (source == "GOOG")
                        {
                            c = double.Parse(data[4].ToString());
                        }
           
                        Price p = new Price { AssetDBId = aid, price = c, date = date };
                        //conversion en euro
                        if (!curr.Equals(Currencies.EUR))
                        {
                            p.priceEur = CurrencyAsset.convertToEuro(p.price, curr, date, context);
                        }
                        else
                        {
                            p.priceEur = p.price;
                        }
                        list_prices.Add(p);
                    }
                }
                list_prices.ForEach(p => context.Prices.Add(p));
                context.SaveChanges();
            }
        }

        public static KeyValuePair<string, string> getCodeQuandl(string symbolBloomberg)
        {
            switch (symbolBloomberg)
            {
                case "AAPL:US":
                    return new KeyValuePair<string, string>("YAHOO", "AAPL");
                case "SAN:SM":
                    return new KeyValuePair<string,string>("YAHOO", "MC_SAN");
                case "939:HK":
                    return new KeyValuePair<string, string>("YAHOO", "HK_0939");
                case "941:HK":
                    return new KeyValuePair<string, string>("YAHOO", "HK_0941");
                case "CSGN:VX":
                    return new KeyValuePair<string, string>("GOOG", "VTX_CSGN");
                case "XOM:US":
                    return new KeyValuePair<string, string>("YAHOO", "XOM");
                case "HSBA:LN":
                    return new KeyValuePair<string, string>("YAHOO", "L_HSBA");
                case "1398:HK":
                    return new KeyValuePair<string, string>("YAHOO", "HK_1398");
                case "JNJ:US":
                    return new KeyValuePair<string, string>("YAHOO", "JNJ");
                case "MSFT:US":
                    return new KeyValuePair<string, string>("YAHOO", "MSFT");
                case "NESN:VX":
                    return new KeyValuePair<string, string>("GOOG", "VTX_NESN");
                case "NOVN:VX":
                    return new KeyValuePair<string, string>("GOOG", "VTX_NOVN");
                case "PG":
                    return new KeyValuePair<string, string>("YAHOO", "PG");
                case "ROG:VX":
                    return new KeyValuePair<string, string>("GOOG", "VTX_ROG");
                case "SAN:FP":
                    return new KeyValuePair<string, string>("GOOG", "EPA_SAN");
                case "SIE:GR":
                    return new KeyValuePair<string, string>("YAHOO", "F_SIE");///
                case "TEF:SM":
                    return new KeyValuePair<string, string>("YAHOO", "MC_TEF");
                case "FP:FP":
                    return new KeyValuePair<string, string>("YAHOO", "PA_FP");
                case "UBSG:VX":
                    return new KeyValuePair<string, string>("YAHOO", "UBS"); //
                case "VOD:LN":
                    return new KeyValuePair<string, string>("YAHOO", "L_VOD");
                default:
                    return new KeyValuePair<string,string>();
            }
        }

        public static List<KeyValuePair<string, string>> getQuandlCodes(List<string> bloombergSymbol)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            foreach (string symbol in bloombergSymbol)
            {
                list.Add(getCodeQuandl(symbol));
            }
            return list;
        }

        public static void storeAllInDB(List<string> bloombergSymbol, qpcptfaw context, DateTime start, DateTime end)
        {
            string json;
            List<KeyValuePair<string, string>> listQuandlCodes = getQuandlCodes(bloombergSymbol);
            List<string> list_symbols_db = Access.getAllEquitiesSymbol(context);
            foreach (KeyValuePair<string, string> pair in listQuandlCodes)
            {
                if (!list_symbols_db.Contains(pair.Value))
                {
                    json = GetJsonData(pair.Key, pair.Value, start, end);
                    storeInDB(json, context, pair.Key, pair.Value);
                }
                else
                {
                    DateTime lastdate = Access.GetLastData(pair.Value);
                    if (lastdate < start)
                    {
                        if (lastdate == DBInitialisation.DBstart)
                        {
                            json = GetJsonData(pair.Key, pair.Value, lastdate, end);
                        }
                        else
                        {
                            json = GetJsonData(pair.Key, pair.Value, start, end);
                        }
                        storeInDB(json, context, pair.Key, pair.Value);
                    }
                    else
                    {
                        if (lastdate < end)
                        {
                            json = GetJsonData(pair.Key, pair.Value, lastdate, end);
                            storeInDB(json, context, pair.Key, pair.Value);
                        }
                    }
                }
            }
        }
    }
}
