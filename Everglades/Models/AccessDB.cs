using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class AccessDB
    {
        private static RandomNormal rand = new RandomNormal();

        // return list of assets names
        public static List<string> Get_Asset_List()
        {
            LinkedList<string> list = new LinkedList<string>();
            list.AddFirst("apple");
            list.AddFirst("pineapple");
            list.AddFirst("cherry");
            list.AddFirst("orange");
            list.AddFirst("lion");
            list.AddFirst("kaboul");
            list.AddFirst("balis");
            list.AddFirst("azfve");
            list.AddFirst("bagoul");
            list.AddFirst("patus");
            return list.ToList();
        }

        // return price of an asset from name
        public static double Get_Asset_Price(String assetName, DateTime date)
        {
            return 100 + rand.NextNormal()*20;
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