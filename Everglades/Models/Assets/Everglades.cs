using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Everglades : IAsset
    {

        string IAsset.getName()
        {
            return "Everglades product";
        }

        // TODO
        double IAsset.getPrice()
        {
            return 78;
        }

        //TODO
        Data IAsset.getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            //TODO
            Data data = new Data();
            data.add(new DataPoint(DateTime.Now - TimeSpan.FromDays(30), 56));
            data.add(new DataPoint(DateTime.Now - TimeSpan.FromDays(15), 124));
            data.add(new DataPoint(DateTime.Now, 78));
            return data;
        }

        //TODO
        double IAsset.getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        //TODO
        double IAsset.getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }


        double IAsset.getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}