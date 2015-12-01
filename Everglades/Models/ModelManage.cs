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
            Hedging_Portfolio = new Portfolio();
            Assets = new Portfolio();
        }

    }
}