using Everglades.Models.HistoricCompute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;


namespace Everglades.Models
{
    public class Everglades : IAsset
    {

        private Currency currency;
        private List<IAsset> underlying_list;

        private double current_price;
        private double[] current_delta;
        private DateTime last_update;

        public Everglades(List<IAsset> underlying_list)
        {
            this.underlying_list = underlying_list;
            currency = new Currency("€");
        }

        public string getName()
        {
            return "Everglades product";
        }

        public LinkedList<DateTime> getObservationDates()
        {
            LinkedList<DateTime> list = new LinkedList<DateTime>();
            // using constructor DateTime(year, month, day)
            list.AddLast(new DateTime(2011, 03, 1)); // initial observation
            list.AddLast(new DateTime(2011, 06, 1)); // observation 1
            list.AddLast(new DateTime(2011, 09, 1)); // observation 2
            list.AddLast(new DateTime(2011, 12, 1)); // observation 3
            list.AddLast(new DateTime(2012, 03, 1)); // observation 4
            list.AddLast(new DateTime(2012, 06, 1)); // observation 5
            list.AddLast(new DateTime(2012, 08, 31)); // observation 6
            list.AddLast(new DateTime(2012, 11, 30)); // observation 7
            list.AddLast(new DateTime(2013, 03, 1)); // observation 8
            list.AddLast(new DateTime(2013, 05, 31)); // observation 9
            list.AddLast(new DateTime(2013, 08, 30)); // observation 10
            list.AddLast(new DateTime(2013, 11, 30)); // observation 11
            list.AddLast(new DateTime(2014, 02, 28)); // observation 12
            list.AddLast(new DateTime(2014, 05, 30)); // observation 13
            list.AddLast(new DateTime(2014, 09, 1)); // observation 14
            list.AddLast(new DateTime(2014, 12, 1)); // observation 15
            list.AddLast(new DateTime(2015, 02, 27)); // observation 16
            list.AddLast(new DateTime(2015, 06, 1)); // observation 17
            list.AddLast(new DateTime(2015, 09, 1)); // observation 18
            list.AddLast(new DateTime(2015, 12, 1)); // observation 19
            list.AddLast(new DateTime(2016, 03, 1)); // observation 20
            list.AddLast(new DateTime(2016, 06, 1)); // observation 21
            list.AddLast(new DateTime(2016, 09, 1)); // observation 22
            list.AddLast(new DateTime(2016, 12, 1)); // observation 23
            list.AddLast(new DateTime(2017, 03, 1)); // observation 24
            return list;
        }

        private void update_current()
        {
            // determine dates to get data for : all observation dates before now + now
            LinkedList<DateTime> dates = new LinkedList<DateTime>();
            foreach (DateTime d in getObservationDates())
            {
                if (d > DateTime.Now)
                {
                    break;
                }
                dates.AddLast(d);
            }
            int nb_day_after = Convert.ToInt32((DateTime.Now - dates.Last.Value).TotalDays); // round to nearest integer (in case of x.9999 -> x and not x+1)
            dates.AddLast(DateTime.Now);
            // create and get data for all arguments
            double[,] historic = new double[underlying_list.Count, dates.Count];
            double[] expected_returns = new double[underlying_list.Count];
            double[] vol = new double[underlying_list.Count];
            double[,] correl;
            int ass_i = 0;
            foreach (IAsset ass in underlying_list)
            {
                int d_i = 0;
                foreach (DateTime d in dates)
                {
                    historic[ass_i, d_i] = ass.getPrice(d);
                    d_i++;
                }
                expected_returns[ass_i] = ass.getCurrency().getInterestRate(DateTime.Now, TimeSpan.FromDays(90));
                vol[ass_i] = ass.getVolatility(DateTime.Now);
                ass_i++;
            }
            // correlation is a bit trickier
            int date_nb_correl = 100;
            int asset_nb = underlying_list.Count;
            double[][] prices = new double[asset_nb][];
            int j = 0;
            foreach (IAsset ass in underlying_list)
            {
                prices[j] = new double[date_nb_correl];
                DateTime titer = DateTime.Now - TimeSpan.FromDays(100);
                for (int i = 0; i < date_nb_correl; i++)
                {
                    prices[j][i] = ass.getPrice(titer);
                    titer += TimeSpan.FromDays(1);
                }
                j++;
            }
            correl = HistoricCorrelation.computeCorrelation(date_nb_correl, asset_nb, prices, vol);
            double r = this.getCurrency().getInterestRate(new DateTime(2011, 03, 1), new DateTime(2013, 03, 1) - new DateTime(2011, 03, 1));
            double r2 = this.getCurrency().getInterestRate(new DateTime(2013, 03, 1), new DateTime(2017, 03, 1) - new DateTime(2013, 03, 1));
            int sampleNb = 5;
            // price
            Wrapping.WrapperEverglades wp = new Wrapping.WrapperEverglades();
            wp.getPriceEverglades(dates.Count, asset_nb, historic, expected_returns, vol, correl, nb_day_after, r, sampleNb);
            this.current_delta = wp.getDelta();
            this.current_price = wp.getPrice();
        }



        // TODO
        public double getPrice()
        {
            // if last update done more than one minute ago, we recalculate
            if ((DateTime.Now - last_update).TotalMinutes > 1.0)
            {
                update_current();
            }
            return current_price;
        }

        //TODO
        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            //TODO
            Data data = new Data();
            data.add(new DataPoint(DateTime.Now - TimeSpan.FromDays(30), 56));
            data.add(new DataPoint(DateTime.Now - TimeSpan.FromDays(15), 124));
            data.add(new DataPoint(DateTime.Now, 78));
            return data;
        }

        //TODO
        public double getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        //TODO
        public Portfolio getDelta()
        {
            // if last update done more than one minute ago, we recalculate
            if ((DateTime.Now - last_update).TotalMinutes > 1.0)
            {
                update_current();
            }
            Portfolio port = new Portfolio(underlying_list);
            int i = 0;
            foreach (IAsset ass in underlying_list)
            {
                port.addAsset(ass, current_delta[i]);
                i++;
            }
            return port;
        }

        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public Currency getCurrency()
        {
            return currency;
        }

    }
}