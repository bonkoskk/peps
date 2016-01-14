using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Wrapping;

namespace Everglades.Models
{
    public class Equity : IAsset
    {
        private string name;
        private Currency currency;

        public Equity(string name, Currency currency)
        {
            this.name = name;
            this.currency = currency;
        }

        public String getName()
        {
            return name;
        }

        public Currency getCurrency()
        {
            return currency;
        }

        public double getPrice()
        {
            return AccessDB.Get_Asset_Price(this.name, DateTime.Now);
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            if (t1 > t2)
            {
                throw new ArgumentOutOfRangeException("Start date must be before end date");
            }
            if (step.TotalDays <= 0)
            {
                throw new ArgumentOutOfRangeException("Step must be strictly positive");
            }
            DateTime t = t1;
            Data data = new Data();
            while (t < t2)
            {
                data.add(new DataPoint(t, AccessDB.Get_Asset_Price(this.name, t)));
                t += step;
            }
            return data;
        }

        public double getPrice(DateTime t)
        {
            return AccessDB.Get_Asset_Price(this.name, t);
        }

        public double getDelta(DateTime t)
        {
            return AccessDB.Get_Asset_Delta(this.name, t);
        }

        public double getVolatility(DateTime t)
        {
            // to calculate volatility, we first calculate an historical volatility on 100 days
            const int N = 100;
            double sum2 = 0;
            double temp;
            DateTime titer = t;
            int i = 0;
            while (titer > t - TimeSpan.FromDays(100))
            {
                temp = ((IAsset)this).getPrice(t);
                sum2 += temp * temp;
                titer -= TimeSpan.FromDays(1);
                i++;
            }
            double histVol = sum2 / (double)(i - 1);
            return histVol;
        }
    }
}