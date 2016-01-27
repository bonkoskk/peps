using Everglades.Models.DataBase;
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
        // wrapping object for c++ / cli functions access (price)
        Wrapping.WrapperEverglades wp;
        private double VLR;
        private Currency currency;
        private List<IAsset> underlying_list;

        public Everglades(List<IAsset> underlying_list)
        {
            wp = new Wrapping.WrapperEverglades();
            this.VLR = 200; //TODO TODO TODO TODO TODO TODO TODO TODO
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
            list.AddLast(new DateTime(2013, 11, 29)); // observation 11
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

        public LinkedList<DateTime> getAnticipatedDates()
        {
            LinkedList<DateTime> list = new LinkedList<DateTime>();
            // using constructor DateTime(year, month, day)
            list.AddLast(new DateTime(2013, 03, 1)); // observation 8
            return list;
        }

        public DateTime getLastDate()
        {
            // using constructor DateTime(year, month, day)
            return new DateTime(2017, 03, 1); // observation 24
        }

        public double getPrice()
        {
            return getPrice(DateTime.Now);
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            Data data = new Data();
            DateTime t = t1;
            while (t < t2)
            {
                data.add(new DataPoint(t, getPrice(t)));
                t += step;
            }
            data.add(new DataPoint(t2, getPrice(t2)));
            return data;
        }

        public double getPrice(DateTime t)
        {
            try
            {
                return AccessDB.getEvergladesPrice(t);
            }
            catch (NoDataException)
            {
                double price = computePrice(t).Item1;
                AccessDB.setEvergladesPrice(t, price);
                return price;
            }
        }

        public Tuple<bool, double> getPayoff(DateTime t)
        {
            LinkedList<DateTime> dates = new LinkedList<DateTime>();
            foreach (DateTime d in getObservationDates())
            {

                if (d > t)
                {
                    break;
                }
                dates.AddLast(d);
            }
            double[,] historic = new double[underlying_list.Count, dates.Count];
            int ass_i = 0;
            foreach (IAsset ass in underlying_list)
            {
                int d_i = 0;
                foreach (DateTime d in dates)
                {
                    historic[ass_i, d_i] = ass.getPrice(d);
                    d_i++;
                }
                ass_i++;
            }
            int nb_dates = dates.Count;
            int nb_asset = this.underlying_list.Count;
            wp.getPayoffEverglades(nb_dates, nb_asset, historic, this.VLR);
            return new Tuple<bool, double>(wp.getPayoffIsAnticipated(), wp.getPayoff());
        }

        public Tuple<double, double[]> computePrice(DateTime t)
        {
            // determine dates to get data for : all observation dates before now + now
            LinkedList<DateTime> dates = new LinkedList<DateTime>();
            foreach (DateTime d in getObservationDates()) 
            {
            
                if (d > t)
                {
                    break;
                }
                dates.AddLast(d);
            }
            
            int nb_day_after = Convert.ToInt32((t - dates.Last.Value).TotalDays); // round to nearest integer (in case of x.9999 -> x and not x+1)
            if (nb_day_after != 0)
            {
                dates.AddLast(t);
            }
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
                expected_returns[ass_i] = ass.getCurrency().getInterestRate(t, TimeSpan.FromDays(90));
                vol[ass_i] = ass.getVolatility(t);
                ass_i++;
            }
            // correlation is a bit trickier
            int asset_nb = underlying_list.Count;
            if (1 == 1)
            {
                correl = new double[asset_nb,asset_nb];
                for(int i = 0; i < asset_nb ; i++) {
                    for (int j = 0; j < asset_nb; j++)
                    {
                        if (i == j)
                        {
                            correl[i, j] = 1;
                        }
                        else
                        {
                            correl[i, j] = 0.0;
                        }
                    }
                }
            } else {
                int date_nb_correl = 100;
                double[][] prices = new double[asset_nb][];
                int j = 0;
                foreach (IAsset ass in underlying_list)
                {
                    prices[j] = new double[date_nb_correl];
                    DateTime titer = t - TimeSpan.FromDays(date_nb_correl);
                    for (int i = 0; i < date_nb_correl; i++)
                    {
                        prices[j][i] = ass.getPrice(titer);
                        titer += TimeSpan.FromDays(1);
                    }
                    j++;
                }
                correl = HistoricCorrelation.computeCorrelation(date_nb_correl, asset_nb, prices, vol);
            }
            double r = this.getCurrency().getInterestRate(new DateTime(2011, 03, 1), new DateTime(2013, 03, 1) - new DateTime(2011, 03, 1));
            int sampleNb = 100;
             
            // price
            wp.getPriceEverglades(dates.Count, asset_nb, historic, expected_returns, vol, correl, nb_day_after, r, sampleNb);
            return new Tuple<double, double[]>(wp.getPrice(), wp.getDelta());
        }

        //TODO
        public Portfolio getDeltaPortfolio()
        {
            return getDeltaPortfolio(DateTime.Now);
        }

        public Portfolio getDeltaPortfolio(DateTime t)
        {
            double[] delta = computePrice(t).Item2;
            Portfolio port = new Portfolio(underlying_list);
            int i = 0;
            foreach (IAsset ass in underlying_list)
            {
                port.addAsset(ass, delta[i]);
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