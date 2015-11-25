using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglades.Models
{

    public interface IAsset
    {
        double Get_Price();
        double[] Get_Price(DateTime t1, DateTime t2, TimeStep step);
        double Get_Price(DateTime t);
        double Get_Delta(DateTime t);
    }
}
