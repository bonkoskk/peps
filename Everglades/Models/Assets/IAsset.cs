using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglades.Models
{

    public interface IAsset
    {
        String getName();
        double getPrice();
        Currency getCurrency();
        Data getPrice(DateTime t1, DateTime t2, TimeSpan step);
        double getPrice(DateTime t);
        double getVolatility(DateTime t);
        double getDelta(DateTime t);
    }
}
