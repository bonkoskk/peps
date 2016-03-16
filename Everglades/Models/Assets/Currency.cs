using AccessBD;
using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Currency : ICurrency
    {
        private Currencies cur_enum;
        
        public Currency(Currencies cur_enum)
        {
            this.cur_enum = cur_enum;
        }

        public Currencies getEnum()
        {
            return cur_enum;
        }

        public String getSymbol()
        {
            switch (cur_enum)
            {
                case Currencies.EUR:
                    return "€";
                case Currencies.GBP:
                    return "£";
                case Currencies.USD:
                    return "$";
                case Currencies.HKD:
                    return "HK$";
                case Currencies.CHF:
                    return "CHF";
                default:
                    return cur_enum.ToString();
            }
        }

        public double getInterestRate(DateTime date)
        {
            // TODO
            return 0.03;
        }

        public double getChangeToEuro(DateTime date)
        {
            throw new NotImplementedException();
            //return 1;
        }

        public string getName()
        {
            return this.cur_enum.ToString();
        }

        public double getPrice()
        {
            return getPrice(DateTime.Today);
        }

        public Currency getCurrency()
        {
            throw new NotImplementedException();
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public double getPrice(DateTime t)
        {
            DateTime tround = new DateTime(t.Year, t.Month, t.Day);
            return Access.GetCurrencyExchangeWithEuro(this.cur_enum, tround);
        }

        public double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getDividend(DateTime t1, DateTime t2)
        {
            // TODO
            return getInterestRate(t1) * (t2 - t1).TotalDays / 365;
        }
    }
}