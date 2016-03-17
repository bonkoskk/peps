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
                case "17899":
                    return Irate.ChinaIR;
                case "IUDVWKA":
                    return Irate.Sterling;
                case "3842":
                    return Irate.LiborCHF;
                case "QS_D_IEUTIO3M":
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
                case Irate.ChinaIR:
                    return new KeyValuePair<string, string>("BCB", "17899"); //à changer
                case Irate.Sterling:
                    return new KeyValuePair<string, string>("BOE", "IUDVWKA");
                case Irate.LiborCHF:
                    return new KeyValuePair<string, string>("BCB", "3842");
                case Irate.Euribor:
                    return new KeyValuePair<string, string>("BOF", "QS_D_IEUTIO3M");
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
                ForexRateDB frdb = new ForexRateDB { irate = current_interestrate };
                //ForexDB fdb = new ForexDB { currency = current_currency };
                context.ForexRates.Add(frdb);
                context.SaveChanges();
            }
            //FIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIlAAAAAAAAAAAAAAAAAAAA YAAAAAAAAAAAAAAAAM
           
            List<ForexRateDB> list_rates = new List<ForexRateDB>();

            double rate = 0;

            //récupère l'id de la devise
            int id = Access.getForexRateIdFromIrate(current_interestrate);
            if (id != -1)
            {
                foreach (var item in datas.Children())
                {
                    JToken[] data = item.ToArray();
                    DateTime date = DateTime.Parse(data[0].ToString());
                    // keyValue = new KeyValuePair<int, DateTime>(id, date);
                    //pourquoi il yavait ça pour le currency
                    if (!Access.ForexRateContainsKey(context, date, id)) //if (!list_pair_db.Contains(keyValue))
                    {
                        rate = double.Parse(data[1].ToString());
                        ForexRateDB f = new ForexRateDB { ForexDBId = id, date = date, rate = rate };
                        list_rates.Add(f);
                    }
                }
            }
            list_rates.ForEach(p => context.ForexRates.Add(p));
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
