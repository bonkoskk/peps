using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Currency
    {
        private string symbol;
        
        public Currency(string symbol)
        {
            this.symbol = symbol;
        } 

        public double getInterestRate(DateTime start, TimeSpan maturity)
        {
            return 0.03;
        }

        public double getChangeToEuro(DateTime date)
        {
            return 1;
        }

        public string getSymbol()
        {
            return symbol;
        }

    }
}