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

        public Equity(string name)
        {
            this.name = name;
        }
        String IAsset.Get_Name()
        {
            return name;
        }

        //TODO
        double IAsset.Get_Price()
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
        }

        //TODO
        Data IAsset.Get_Price(DateTime t1, DateTime t2, TimeSpan step)
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
            // TODO : Database access
            Data data = new Data();
            int test = 123445;
            while (t < t2)
            {
                test += 5465541;
                data.add(new DataPoint(t, test % 488721));
                t += step;
            }
            return data;
        }

        //TODO
        double IAsset.Get_Price(DateTime t)
        {
            return 1000;
        }

        //TODO
        double IAsset.Get_Delta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}