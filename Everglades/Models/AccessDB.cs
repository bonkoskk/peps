using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class AccessDB
    {
        // return list of assets names
        public static List<string> Get_Asset_List()
        {
            LinkedList<string> list = new LinkedList<string>();
            throw new NotImplementedException();
        }

        // return price of an asset from name
        public static double Get_Asset_Price(String assetName)
        {
            throw new NotImplementedException();
        }

        // return delta of an asset from name
        public static double Get_Asset_Delta(String assetName)
        {
            throw new NotImplementedException();
        }


    }
}