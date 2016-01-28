using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public abstract class AssetDB
    {
        [Key]
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
