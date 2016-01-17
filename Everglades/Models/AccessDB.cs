using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TestAccessBD;

namespace Everglades.Models
{
    public class AccessDB
    {
        private static Random Rand = new Random();

        // return list of assets names
        public static List<string> Get_Asset_List()
        {
            return Access.Get_List_Assets();
        }

        // return price of an asset from name
        public static double Get_Asset_Price(String assetName, DateTime date)
        {
            int id = Access.GetIdFromName(assetName);
            Dictionary<string, double> P = Access.Get_Price(id, new DateTime(date.Year, date.Month, date.Day));
            return P["close"];
        }

        // return delta of an asset from name
        public static double Get_Asset_Delta(String assetName, DateTime date)
        {
            throw new NotImplementedException();
        }

        // return interest rate of a particular money
        public static double Get_Interest_Rate(String moneyName, DateTime date)
        {
            return 0.02; // TODO
        }


    }
}