using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class ModelManage
    {
        public List<IAsset> Assets;
        public Portfolio Hedging_Portfolio;

        public ModelManage()
        {
            Assets = new List<IAsset>();
            foreach (string name in AccessDB.Get_Asset_List())
            {
                Assets.Add(new Equity(name));
            }
            Hedging_Portfolio = new Portfolio(Assets);
        }

    }
}