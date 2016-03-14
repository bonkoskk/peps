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
        private Wrapping.WrapperEverglades wp;
        private double VLR;
        private Currency currency;
        private List<IAsset> underlying_list;
        private DateTime Last_Correl_Computation;
        private double[,] Last_Cholesky;
        private double[] Last_Vol;

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

        private Tuple<double[,], double[]> computeCorrelationAndVol(DateTime priceDate, List<String> assetNames, uint date_nb)
        {
            int asset_nb = assetNames.Count;
            // create a list of dates to get price for correlation computing
            int nb_dates_correl = 200;
            List<DateTime> dates_correl = new List<DateTime>();
            int total = 0, real = 0; // current number of date : total also have not taken dates cause week end
            while (real < nb_dates_correl)
            {
                DateTime curr_date = priceDate - TimeSpan.FromDays(total);
                if (!(curr_date.DayOfWeek == DayOfWeek.Saturday || curr_date.DayOfWeek == DayOfWeek.Sunday))
                {
                    dates_correl.Add(curr_date);
                    real++;
                }
                total++;
            }
            // get the prices from database
            Dictionary<Tuple<String, DateTime>, double> hist_correl = AccessDB.Get_Asset_Price_Eur(assetNames, dates_correl);
            // transform the Tuple format to double[,] format
            double[,] hist_correl_double = new double[asset_nb, nb_dates_correl];
            int k = 0;
            foreach (IAsset ass in underlying_list)
            {
                int j = 0;
                foreach (DateTime d in dates_correl)
                {
                    hist_correl_double[k, j] = hist_correl[new Tuple<String, DateTime>(ass.getName(), d)];
                    j++;
                }
                k++;
            }
            // compute correl and vol using C++ functions
            Wrapping.Tools tools = new Wrapping.Tools();
            double[,] correl = new double[asset_nb, asset_nb];
            double[] vol = new double[asset_nb];
            tools.getCorrelAndVol(nb_dates_correl, asset_nb, hist_correl_double, correl, vol);
            return new Tuple<double[,], double[]>(correl, vol);
        }


        public Tuple<double, double[]> computePrice(DateTime t)
        {
            ModelManage.timers.start("Everglades pre-pricing");
            int asset_nb = underlying_list.Count;
            // risk-free rate
            double r = 0.04;//this.getCurrency().getInterestRate(new DateTime(2011, 03, 1), new DateTime(2013, 03, 1) - new DateTime(2011, 03, 1));
            // determine dates to get data for : all observation dates before now + now
            LinkedList<DateTime> dates = new LinkedList<DateTime>();
            DateTime priceDate = new DateTime(t.Year, t.Month, t.Day);
            foreach (DateTime d in getObservationDates()) 
            {
            
                if (d > priceDate)
                {
                    break;
                }
                dates.AddLast(d);
            }
            int nb_day_after = 0;
            if (dates.Count == 0)
            {
                dates.AddLast(new DateTime(priceDate.Year, priceDate.Month, priceDate.Day));
            }
            else
            {
                nb_day_after = Convert.ToInt32((priceDate - dates.Last.Value).TotalDays); // round to nearest integer (in case of x.9999 -> x and not x+1)
                if (nb_day_after > 0)
                {
                    dates.AddLast(priceDate);
                }
            
            }
            // create and get data for all arguments
            double[,] historic = new double[asset_nb, dates.Count];
            double[] expected_returns = new double[asset_nb];
            double[] vol = new double[asset_nb];
            double[,] correl = new double[asset_nb, asset_nb];
            double[,] cholesky;

            // get historic data for all functions
            ModelManage.timers.start("Everglades historic data");
            List<String> assetNames = new List<String>();
            foreach (IAsset ass in underlying_list)
            {
                assetNames.Add(ass.getName());
            }
            Dictionary<Tuple<String, DateTime>, double> hist;

            if (underlying_list.First() is Equity)
            {
                // get prices in BD at constatation dates
                hist = AccessDB.Get_Asset_Price_Eur(assetNames, dates.ToList());
                // compute or get correlation and vol
                if ((priceDate - Last_Correl_Computation).TotalDays > 30)
                {
                    uint nb_dates_correl = 200;
                    Tuple<double[,], double[]> temp = computeCorrelationAndVol(priceDate, assetNames, nb_dates_correl);
                    correl = temp.Item1;
                    vol = temp.Item2;
                    cholesky = wp.factCholesky(correl, asset_nb);
                    Last_Cholesky = cholesky;
                    Last_Vol = vol;
                }
                else
                {
                    cholesky = Last_Cholesky;
                    vol = Last_Vol;
                }
                
            }
            else
            {
                hist = new Dictionary<Tuple<string, DateTime>, double>();
                correl = new double[asset_nb, asset_nb];
                for (int i = 0; i < asset_nb; i++)
                {
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
                    expected_returns[i] = r;
                }
                cholesky = correl; // cholesky fact of identity is identity
            }
            
            int ass_i = 0;
            foreach (IAsset ass in underlying_list)
            {
                int d_i = 0;
                foreach (DateTime d in dates)
                {
                    if (ass is Equity)
                    {
                        var key = new Tuple<String, DateTime>(ass.getName(), d);
                        historic[ass_i, d_i] = hist[key];
                    }
                    else
                    {
                        historic[ass_i, d_i] = ass.getPrice(d);
                    }
                    d_i++;
                }
                expected_returns[ass_i] = r; //ass.getCurrency().getInterestRate(t, TimeSpan.FromDays(90));
                vol[ass_i] = ass.getVolatility(t);
                ass_i++;
            }

            ModelManage.timers.stop("Everglades historic data");


            int sampleNb = 1000; 
            // pricing
            wp.getPriceEverglades(dates.Count, asset_nb, historic, expected_returns, vol, cholesky, nb_day_after, r, sampleNb);
            
            //wp.getPriceEverglades(dates.Count, asset_nb, historic, expected_returns, vol, correl, nb_day_after, r, sampleNb);

            /*
            DateTime T = new DateTime(2017, 03, 14);
            double tau = (T - t).TotalDays / 365;


            Wrapping.WrapperDebugVanilla wp = new Wrapping.WrapperDebugVanilla();
            wp.getPriceVanilla(0, asset_nb, historic[0, historic.Length], expected_returns, vol, correl, tau, r, sampleNb, historic[0, historic.Length]);
            */
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