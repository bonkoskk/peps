using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Everglades : IAsset
    {

        public string getName()
        {
            return "Everglades product";
        }

        // TODO
        public double getPrice()
        {
            return 78;
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
            throw new NotImplementedException();
        }

    }
}