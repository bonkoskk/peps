using AccessBD;
using Everglades.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wrapping;

namespace Everglades.Models.Assets
{
    public abstract class AQuanto : IDerivative
    {
        protected IAsset underlying;
        protected double strike;
        protected DateTime maturity;
        protected double volatility;
        protected static Currency currency = new Currency(Currencies.EUR);
        protected static WrapperQuanto wp = new WrapperQuanto();

        public List<Param> getParam()
        {
            List<Param> param = new List<Param>();
            param.Add(new Param("underlying", ParamType._equity));
            param.Add(new Param("strike", ParamType._double));
            param.Add(new Param("maturity", ParamType._date));
            return param;
        }

        public void setParam(List<Param> param)
        {
            Param[] P = param.ToArray();
            underlying = ModelManage.instance.Assets.Find(x => String.Compare(x.getName(), P[0].getString()) == 0);
            strike = P[1].getDouble();
            maturity = P[2].getDate();
        }

        public double getPrice()
        {
            return getPrice(DateTime.Now);
        }

        public Currency getCurrency()
        {
            return currency;
        }

        public abstract string getName();
        public abstract double getDelta(DateTime t);
        public abstract double getPrice(DateTime t);
        public abstract Data getPrice(DateTime t1, DateTime t2, TimeSpan step);
        public abstract string getType();
        public abstract double getVolatility(DateTime t);



        public double getDividend(DateTime t1, DateTime t2)
        {
            return 0;
        }


        public double getPriceEuro(DateTime t)
        {
            return getPrice(t);
        }

        protected static Tuple<double[,], double[]> computeCorrelationAndVol(DateTime priceDate, IAsset underlying, ICurrency cur, int date_nb)
        {
            List<DateTime> dates_correl = new List<DateTime>();
            int total = 0, real = 0; // current number of date : total also have not taken dates cause week end
            while (real < date_nb)
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
            Dictionary<DateTime, double> hist_correl = AccessDB.Get_Asset_Price(underlying.getName(), dates_correl);
            // transform the Tuple format to double[,] format
            double[,] hist_correl_double = new double[2, date_nb];
            int j = 0;
            foreach (DateTime d in dates_correl)
            {
                hist_correl_double[0, j] = hist_correl[d];
                j++;
            }
            j = 0;
            foreach (DateTime d in dates_correl)
            {
                hist_correl_double[1, j] = 1 / cur.getChangeToEuro(d);
                j++;
            }

            // compute correl and vol using C++ functions
            Wrapping.Tools tools = new Wrapping.Tools();
            double[,] correl = new double[2, 2];
            double[] vol = new double[2];
            tools.getCorrelAndVol(date_nb, 2, hist_correl_double, correl, vol);
            return new Tuple<double[,], double[]>(correl, vol);
        }


    }
}
