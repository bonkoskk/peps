using Everglades.Models.DataBase;
using Everglades.Models.HistoricCompute;
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
            Console.WriteLine(name);
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
            return getPrice(DateTime.Now);
        }

        public Double[,] getPriceDouble(DateTime t1, DateTime t2, TimeSpan step)
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
            List<DateTime> dates = new List<DateTime>();
            while (t < t2)
            {
                dates.Add(t);
                t += step;
            }

            Dictionary<DateTime, double> prices = AccessDB.Get_Asset_Price(this.name, dates);
            double[,] data = new double[1, dates.Count];
            int i = 0;
            foreach (DateTime d in dates)
            {
                data[1, i] = prices[d];
                i++;
            }
            return data;
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
            List<DateTime> dates = new List<DateTime>();
            while (t < t2)
            {
                dates.Add(t);
                t += step;
            }

            Dictionary<DateTime, double> prices = AccessDB.Get_Asset_Price(this.name, dates);
            Data data = new Data();
            foreach(DateTime d in dates)
            {
                if (prices.ContainsKey(d))
                {
                    data.add(new DataPoint(d, prices[d]));
                }
                else
                {
                    data.add(new DataPoint(d, AccessDB.Get_Asset_Price(this.name, d)));
                }
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
            Wrapping.Tools tools = new Wrapping.Tools();

            int nb_dates = 15;

            double[,] price = getPriceDouble(t, t - TimeSpan.FromDays(nb_dates), TimeSpan.FromDays(1));

            double[,] correl = new double[1, 1];
            double[] vol = new double[1];
            tools.getCorrelAndVol(nb_dates, 1, price, correl, vol);
            return vol[0];
        }

        public double getCovariance(IAsset a, DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}