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

        public static Currencies getCurrency(Irate ir)
        {
            switch (ir)
            {
                case Irate.Euribor:
                    return Currencies.EUR;
                case Irate.Hibor:
                    return Currencies.HKD;
                case Irate.LiborCHF:
                    return Currencies.CHF;
                case Irate.LiborGBP:
                    return Currencies.GBP;
                case Irate.LiborUSD:
                    return Currencies.USD;
                default:
                    return Currencies.EUR;
            }
        }
    }
}
