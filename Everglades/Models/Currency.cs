using AccessBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Currency
    {
        private string symbol;
        private Currencies cur_enum;
        
        public Currency(Currencies cur_enum)
        {
            this.symbol = cur_enum.ToString();
            this.cur_enum = cur_enum;
        }

        public Currencies getEnum()
        {
            return cur_enum;
        }

        public double getInterestRate(DateTime start, TimeSpan maturity)
        {
            //return 0.03;
            throw new NotImplementedException();
        }

        public double getChangeToEuro(DateTime date)
        {
            throw new NotImplementedException();
            //return 1;
        }

        public string getSymbol()
        {
            return symbol;
        }

    }
}