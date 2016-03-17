using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public enum Currencies
    {
        USD,
        CHF,
        HKD,
        GBP,
        EUR
    }

    public class CurrencyAsset
    {
        public static Currencies getCurrencyOf(string assetname)
        {
            switch (assetname)
            {
                case "AAPL: Apple Inc. -":
                    return Currencies.USD;
                case "SANTANDER - Madrid SE C.A.T.S.":
                    return Currencies.EUR;
                case "0939: CCB":
                    return Currencies.HKD;
                case "0941: CHINA MOBILE":
                    return Currencies.HKD;
                case "Credit Suisse Group AG (CSGN)":
                    return Currencies.CHF;
                case "XOM: Exxon Mobil Corporation Common  -":
                    return Currencies.USD;
                case "HSBA: HSBC HLDG - London Stock Exchange":
                    return Currencies.GBP;
                case "1398: ICBC":
                    return Currencies.HKD;
                case "JNJ: Johnson & Johnson Common Stock -":
                    return Currencies.USD;
                case "MSFT: Microsoft Corporation -":
                    return Currencies.USD;
                case "Nestle SA (NESN)":
                    return Currencies.CHF;
                case "Novartis AG (NOVN)":
                    return Currencies.CHF;
                case "PG: Procter & Gamble Company (The)  -":
                    return Currencies.USD;
                case "Roche Holding Ltd. (ROG)":
                    return Currencies.CHF;
                case "SAN: SANOFI - Paris Stock Exchange":
                    return Currencies.EUR;
                case "SIE: SIEMENS N - Frankfurt Stock Exchange":
                    return Currencies.EUR;///
                case "TEF: TELEFONICA - Madrid SE C.A.T.S.":
                    return Currencies.EUR;
                case "FP: TOTAL - Paris Stock Exchange":
                    return Currencies.EUR;
                case "UBS: UBS AG Common Stock -":
                    return Currencies.CHF; //
                case "VOD: VODAFONE GRP - London Stock Exchange":
                    return Currencies.GBP;
                case "Everglades":
                    return Currencies.EUR;
                default:
                    return Currencies.EUR;
            }
        }

        public static double convertToEuro(double price, Currencies currency, DateTime date, qpcptfaw context){
            if (currency.Equals(Currencies.EUR)) return price;
            double rate = 1;
            bool isException = true;
            DateTime datelocal = date;
            while (isException)
            {
                try
                {
                    rate = Access.getExchangeRate(currency, datelocal, context);
                    isException = false;
                }
                catch (Exception)
                {
                    datelocal = datelocal.AddDays(-1);
                }    
            }
                
            return price/rate;
        }

        public static Dictionary<string, double> convertToEuro(Dictionary<string, double> prices, Currencies currency, DateTime date, qpcptfaw context)
        {
            if (currency.Equals(Currencies.EUR)) return prices;
            Dictionary<string, double> p_converti = new Dictionary<string, double>(); 
            double rate = 1;
            bool isException = true;
            DateTime datelocal = date;
            while (isException)
            {
                try
                {
                    rate = Access.getExchangeRate(currency, datelocal, context);
                    isException = false;
                }
                catch (Exception)
                {
                    datelocal = datelocal.AddDays(-1);
                }
            }
            foreach (KeyValuePair<string,double> p in prices)
            {
                p_converti.Add(p.Key, p.Value / rate);    
            }
            return p_converti;
        }

    }
}
