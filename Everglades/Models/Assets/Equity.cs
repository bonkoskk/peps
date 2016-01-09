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

        double IAsset.Get_Price()
        {
            return AccessDB.Get_Asset_Price(this.name, DateTime.Now);
        }

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
            Data data = new Data();
            while (t < t2)
            {
                data.add(new DataPoint(t, AccessDB.Get_Asset_Price(this.name, t)));
                t += step;
            }
            return data;
        }

        double IAsset.Get_Price(DateTime t)
        {
            return AccessDB.Get_Asset_Price(this.name, t);
        }

        double IAsset.Get_Delta(DateTime t)
        {
            return AccessDB.Get_Asset_Delta(this.name, t);
        }
    }
}