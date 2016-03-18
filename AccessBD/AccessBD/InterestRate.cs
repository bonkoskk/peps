using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public enum Irate
    {
        LiborUSD,
        LiborCHF,
        Hibor,
        LiborGBP,
        Euribor
    }

    class InterestRate
    {

        public static Irate getInteresrRateOf(string assetname)
        {
            switch (assetname)
            {
                case "AAPL: Apple Inc. -":
                    return Irate.LiborUSD;
                case "SANTANDER - Madrid SE C.A.T.S.":
                    return Irate.Euribor;
                case "0939: CCB":
                    return Irate.Hibor;
                case "0941: CHINA MOBILE":
                    return Irate.Hibor;
                case "Credit Suisse Group AG (CSGN)":
                    return Irate.LiborCHF;
                case "XOM: Exxon Mobil Corporation Common  -":
                    return Irate.LiborUSD;
                case "HSBA: HSBC HLDG - London Stock Exchange":
                    return Irate.LiborGBP;
                case "1398: ICBC":
                    return Irate.Hibor;
                case "JNJ: Johnson & Johnson Common Stock -":
                    return Irate.LiborUSD;
                case "MSFT: Microsoft Corporation -":
                    return Irate.LiborUSD;
                case "Nestle SA (NESN)":
                    return Irate.LiborCHF;
                case "Novartis AG (NOVN)":
                    return Irate.LiborCHF;
                case "PG: Procter & Gamble Company (The)  -":
                    return Irate.LiborUSD;
                case "Roche Holding Ltd. (ROG)":
                    return Irate.LiborCHF;
                case "SAN: SANOFI - Paris Stock Exchange":
                    return Irate.Euribor;
                case "SIE: SIEMENS N - Frankfurt Stock Exchange":
                    return Irate.Euribor;
                case "TEF: TELEFONICA - Madrid SE C.A.T.S.":
                    return Irate.Euribor;
                case "FP: TOTAL - Paris Stock Exchange":
                    return Irate.Euribor;
                case "UBS: UBS AG Common Stock -":
                    return Irate.LiborCHF;
                case "VOD: VODAFONE GRP - London Stock Exchange":
                    return Irate.LiborGBP;
                case "Everglades":
                    return Irate.Euribor;
                default:
                    return Irate.Euribor;
            }
        }
    }
}
