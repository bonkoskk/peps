﻿using Everglades.Models.HistoricCompute;
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

            // debug kevin
            // doit retourner 6.51
            WrapperQuanto wc = new WrapperQuanto();

            //wc.getPriceOptionBarrier(50000, 1, 100, 105, 0.25, 0.02, 100, 90);


            if (String.Compare(name, "orange") == 0)
            {
                wc.getPricePutQuanto(100, 1.2, 100, 0.02, 0.03, 0.2, 0.2, 0.1, 1);
                //wc.getPriceOptionBarrier(50000, 1, 100, 105, 0.25, 0.02, 100, 90);
                return wc.getPrice();
            }
            
            return 100;
            
            //return wc.getPrice();
            //return AccessDB.Get_Asset_Price(this.name, DateTime.Now);
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
            // number of observations dates (1 day separated)
            int date_nb = 100;
            // get prices for these days
            DateTime titer = t;
            Double[] prices = new Double[date_nb];
            for(int i=0; i<100; i++)
            {
                prices[i] = this.getPrice(titer);
                titer -= TimeSpan.FromDays(1);
            }
            // compute and return historic volatility
            return HistoricVolatility.compute(date_nb, prices);
        }
    }
}