using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuandlCS.Connection;
using QuandlCS.Requests;
using QuandlCS.Types;
using QuandlCS.Interfaces;
using Newtonsoft.Json.Linq;

namespace AccessBD
{
    class QuandlDataExchange
    {
        public static string GetJsonData(string source, string codeQuandl, DateTime start, DateTime end)
        {
            QuandlDownloadRequest request = new QuandlDownloadRequest();
            request.APIKey = "1ztyfXyfPBTeqfshhBdE";
            request.Datacode = new Datacode(source, codeQuandl);
            request.Format = FileFormats.JSON;
            request.Frequency = Frequencies.Daily;
            request.Transformation = Transformations.None;
            request.StartDate = start;
            request.EndDate = end;

            IQuandlConnection connection = new QuandlConnection();
            return connection.Request(request);

        }


        public static Currencies getCurrencyFromQuandlCode(string code){
            switch (code)
            {
                case "EURHKD":
                    return Currencies.HKD;
                case "EURCHF":
                    return Currencies.CHF;
                case "EURUSD":
                    return Currencies.USD;
                case "EURGBP":
                    return Currencies.GBP;
                default :
                    return Currencies.EUR;
            }
        }

        public static KeyValuePair<string, string> getCodeQuandl(Currencies currency)
        {
            switch (currency)
            {
                case Currencies.USD:
                    return new KeyValuePair<string, string>("ECB", "EURUSD");
                case Currencies.HKD:
                    return new KeyValuePair<string, string>("ECB", "EURHKD");
                case Currencies.GBP:
                    return new KeyValuePair<string, string>("ECB", "EURGBP");
                case Currencies.CHF:
                    return new KeyValuePair<string, string>("ECB", "EURCHF");
                default:
                    return new KeyValuePair<string, string>();
            }
        }

        public static void storeInDB(string json, qpcptfaw context, string source, string code)
        {
            JObject jObj = JObject.Parse(json);
            JArray datas = (JArray)jObj["data"];

            //List<Currencies> list_currencies_db;
            //tous les currencies présents dans la BD
            /*try
            {
                list_currencies_db = Access.getAllCurrencies();
            }catch (Exception){
                list_currencies_db = new List<Currencies>();
            }*/

            Currencies current_currency = getCurrencyFromQuandlCode(code);

            //Si la BD ne contient pas ce currency, on le crée
            if (!Access.CurrenciesContains(current_currency))//!list_currencies_db.Contains(current_currency))
            {
                ForexDB fdb = new ForexDB{forex = current_currency, name = code};
                context.Assets.Add(fdb);
                context.SaveChanges();
            }

            //List<KeyValuePair<int, DateTime>> list_pair_db = Access.getAllForexRateKey(context);
            //KeyValuePair<int, DateTime> keyValue;
            List<Price> list_rates = new List<Price>();

            double rate = 0;

            //récupère l'id de la devise
            int id = Access.getForexIdFromCurrency(current_currency);
            if (id != -1)
            {
                foreach (var item in datas.Children())
                {
                    JToken[] data = item.ToArray();
                    DateTime date = DateTime.Parse(data[0].ToString());
                   // keyValue = new KeyValuePair<int, DateTime>(id, date);
                   if (!Access.ForexRateContainsKey(context, date, id)) //if (!list_pair_db.Contains(keyValue))
                    {
                        rate = double.Parse(data[1].ToString());
                        Price f = new Price {price = rate, priceEur = 1/rate, date = date, AssetDBId = id};
                        list_rates.Add(f);
                    }
                }
            }
            list_rates.ForEach(p => context.Prices.Add(p));
            context.SaveChanges();
        }

        

        public static void storeAllInDB(List<Currencies> currencies, qpcptfaw context, DateTime start, DateTime end)
        {
            string json;
            List<KeyValuePair<string, string>> list_quandl_codes = new List<KeyValuePair<string,string>>();
            foreach(Currencies c in currencies) list_quandl_codes.Add(getCodeQuandl(c));

            /*List<Currencies> list_currencies_db;
            try
            {
                list_currencies_db = Access.getAllCurrencies();
            }
            catch (Exception)
            {
                list_currencies_db = new List<Currencies>();
            }*/

            foreach (KeyValuePair<string, string> pair in list_quandl_codes)
            {
                Currencies c = getCurrencyFromQuandlCode(pair.Value);
                if (!Access.CurrenciesContains(c))//!list_currencies_db.Contains(c))
                {
                    json = GetJsonData(pair.Key, pair.Value, start, end);
                    storeInDB(json, context, pair.Key, pair.Value);
                }
                else
                {
                    DateTime lastdate = Access.GetLastData(c);
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
