using AccessBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Everglades.Models.Assets
{
    public class CurrencySimulated : ICurrency
    {
        private Currencies cur_enum;
        private double sigma;
        private double r;
        private double rforeign;
        private RandomNormal rand;

        private Dictionary<DateTime, double> prices;
        private double lastPrice;
        private DateTime lastDate;

        public CurrencySimulated(Currencies cur_enum, RandomNormal rand)
        {
            this.cur_enum = cur_enum;
            this.r = 0.04;
            this.rforeign = 0.01 + 0.05 * rand.NextDouble();
            this.sigma = 0.1;
            this.lastPrice = 1;
            this.lastDate = DateTime.MinValue;
            this.rand = rand;
            this.prices = new Dictionary<DateTime, double>();
        }

        public Currencies getEnum()
        {
            return cur_enum;
        }

        public double getChangeToEuro(DateTime date)
        {
            if (prices.ContainsKey(date)) {
                return prices[date];
            }
            else if (lastDate == DateTime.MinValue)
            {
                lastDate = date;
                lastPrice = rand.NextDouble() * 5;
            }
            else
            {
                double T = (date - lastDate).TotalDays / 365; // time in year
                double WT = Math.Sqrt(T) * rand.NextNormal();
                lastPrice = lastPrice * Math.Exp((r - rforeign - sigma * sigma / 2) * T + sigma * WT);
                lastDate = date;
            }
            prices[date] = lastPrice;
            return lastPrice;
        }

        public string getName()
        {
            return cur_enum.ToString();
        }

        public double getPrice()
        {
            throw new NotImplementedException();
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
            return getChangeToEuro(t);
        }

        public double getVolatility(DateTime t)
        {
            return sigma;
        }

        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public string getSymbol()
        {
            return this.cur_enum.ToString();
        }

        public double getInterestRate(DateTime date)
        {
            return rforeign;
        }

        public double getDividend(DateTime t1, DateTime t2)
        {
            return getPrice(t2) * (Math.Exp(rforeign * (t2 - t1).TotalDays / 365) - 1);
        }
    }
}