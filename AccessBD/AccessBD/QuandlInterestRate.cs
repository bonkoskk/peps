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
    class QuandlInterestRate
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

        public static Irate getInterestFromQuandlCode(string code)
        {
            switch (code)
            {
                case "USD3MTD156N":
                    return Irate.LiborUSD;
                case "HIB1M":
                    return Irate.Hibor;
                case "GBP3MTD156N":
                    return Irate.LiborGBP;
                case "CHF3MTD156N":
                    return Irate.LiborCHF;
                case "EUR3MTD156N":
                    return Irate.Euribor;
                default:
                    return Irate.Euribor;
            }
        }

        public static KeyValuePair<string, string> getCodeQuandl(Irate interestRate)
        {
            switch (interestRate)
            {
                case Irate.LiborUSD:
                    return new KeyValuePair<string, string>("FRED", "USD3MTD156N");
                case Irate.Hibor:
                    return new KeyValuePair<string, string>("WSJ", "HIB1M"); 
                case Irate.LiborGBP:
                    return new KeyValuePair<string, string>("FRED", "GBP3MTD156N");
                case Irate.LiborCHF:
                    return new KeyValuePair<string, string>("FRED", "CHF3MTD156N");
                case Irate.Euribor:
                    return new KeyValuePair<string, string>("FRED", "EUR3MTD156N");
                default:
                    return new KeyValuePair<string, string>();
            }
        }

        public static void storeInDB(string json, qpcptfaw context, string source, string code)
        {
            JObject jObj = JObject.Parse(json);
            JArray datas = (JArray)jObj["data"];
            //List<Irate> listInterestRate_db;
            //List<Currencies> list_currencies_db;
            Irate current_interestrate = getInterestFromQuandlCode(code);
            //Currencies current_currency = getCurrencyFromQuandlCode(code);

            //Si la BD ne contient pas ce currency, on le crée
            if (!Access.InterestRateContains(current_interestrate))//!list_currencies_db.Contains(current_currency))
            {
                RateDB rate = new RateDB { rate = current_interestrate, name = current_interestrate.ToString()};
                //ForexDB fdb = new ForexDB { currency = current_currency };
                context.InteresRatesType.Add(rate);
                context.SaveChanges();
            }
            //FIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIlAAAAAAAAAAAAAAAAAAAA YAAAAAAAAAAAAAAAAM
           
            List<RateDBValue> list_rates = new List<RateDBValue>();

            double value = 0;

            //récupère l'id de la devise
            int id = Access.getInterestRateIdFromIrate(current_interestrate);
            if (id != -1)
            {
                foreach (var item in datas.Children())
                {
                    JToken[] data = item.ToArray();
                    DateTime date = DateTime.Parse(data[0].ToString());
                    // keyValue = new KeyValuePair<int, DateTime>(id, date);
                    //pourquoi il yavait ça pour le currency
                    if (!Access.InterestRatesContainsKey(context, date, id)) //if (!list_pair_db.Contains(keyValue))
                    {
                        value = double.Parse(data[1].ToString());
                        RateDBValue r = new RateDBValue { date = date, RateDBId = id, value = value};
                        list_rates.Add(r);
                    }
                }
            }
            list_rates.ForEach(p => context.Rates.Add(p));
            context.SaveChanges();
        }

        public static void storeAllInDB(List<Irate> interestRate, qpcptfaw context, DateTime start, DateTime end)
        {
            string json;
            List<KeyValuePair<string, string>> list_quandl_codes = new List<KeyValuePair<string, string>>();
            foreach (Irate ir in interestRate) list_quandl_codes.Add(getCodeQuandl(ir));

            foreach (KeyValuePair<string, string> pair in list_quandl_codes)
            {
                Irate ir = getInterestFromQuandlCode(pair.Value);
                if (!Access.InterestRateContains(ir))
                {
                    json = GetJsonData(pair.Key, pair.Value, start, end);
                    storeInDB(json, context, pair.Key, pair.Value);
                }
                else
                {
                    DateTime lastdate = Access.GetLastData(ir);
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
