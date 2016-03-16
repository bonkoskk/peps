using AccessBD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public interface ICurrency : IAsset
    {
        Currencies getEnum();
        String getSymbol();
        double getInterestRate(DateTime date);
        double getChangeToEuro(DateTime date);
    }
}