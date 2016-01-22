using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public class AssetSimulated : IAsset
    {

        private IAsset real;
        private Dictionary<DateTime, double> prices;
        private double sigma;
        private double r;
        private DateTime first_date;

        /**
         * Create a simulated asset from a real one.
         * It extract the name of real asset for "fake" one
         * and the first price (at first date) and simulate
         * at all dates from dates_simul.
         * 
         * getPrice(t) with t from dates_simul will then return
         * a simulated price
         * getPrice(t) with t before first date from dates_simul
         * will return real price of asset
         * getPrice(t) with all others date will throw exception
         **/
        public AssetSimulated(IAsset real, LinkedList<DateTime> dates_simul, RandomNormal rand)
        {
            this.real = real;
            prices = new Dictionary<DateTime, double>();
            first_date = dates_simul.First();
            r = 0.04;
                //real.getCurrency().getInterestRate(first_date, TimeSpan.Zero);
            // TODO
            //sigma = real.getVolatility(first_date);
            sigma = 0.2;
            double S0 = real.getPrice(first_date);
            foreach (DateTime date in dates_simul)
            {
                double T = (date - first_date).TotalDays / 365; // time in year
                double WT = Math.Sqrt(T) * rand.NextNormal();
                prices[date] = S0 * Math.Exp((r - sigma * sigma / 2) * T + sigma * WT);
            }
        }

        public string getName()
        {
            return "Simulated " + real.getName();
        }

        public double getPrice()
        {
            return getPrice(DateTime.Now);
        }

        public Currency getCurrency()
        {
            return real.getCurrency();
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public double getPrice(DateTime t)
        {
            if (t < first_date)
            {
                return real.getPrice(t);
            }
            else
            {
                return prices[t];
            }
        }

        public double getVolatility(DateTime t)
        {
            return sigma;
        }

        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}