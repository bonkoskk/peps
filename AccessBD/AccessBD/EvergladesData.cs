using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public class EvergladesData
    {
        public static void addEverglades()
        {
            using (var context = new qpcptfaw())
            {
                if (context.Everglades.Count()==0)
                {
                    EvergladesDB e = new EvergladesDB { name = "Everglades"};
                    context.Assets.Add(e);
                    context.SaveChanges();
                    AssetDB.assetCounter();
                }
            }
        }
    }
}
