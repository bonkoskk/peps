using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everglades.Models
{

    public interface IAsset
    {
        String Get_Name();
        double Get_Price();
        Data Get_Price(DateTime t1, DateTime t2, TimeSpan step);
        double Get_Price(DateTime t);
        double Get_Delta(DateTime t);
    }
}
