using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class ModelManage
    {
        public Portfolio Hedging_Portfolio;
        public Portfolio Assets;

        public ModelManage()
        {
            Assets = new Portfolio();
            Hedging_Portfolio = new Portfolio(Assets);
        }

    }
}