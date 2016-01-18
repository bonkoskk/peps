using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestAccessBD
{
    public abstract class AssetDB
    {
        public int AssetDBId { get; set; }
        public string name { get; set; }

        public static int nbAssets{get; set;}

        public static int assetCounter()
        {
            nbAssets++;
            return nbAssets;
        }
    }
}
