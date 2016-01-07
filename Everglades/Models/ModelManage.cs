using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class ModelManage
    {
        public List<IAsset> Assets;
        public double cash;
        public Everglades everg;
        public Portfolio Hedging_Portfolio;
        public List<Operation.Operation> Operations_History;

        public ModelManage()
        {
            cash = 10000;
            Assets = new List<IAsset>();
            foreach (string name in AccessDB.Get_Asset_List())
            {
                Assets.Add(new Equity(name));
            }
            everg = new Everglades();
            Hedging_Portfolio = new Portfolio(Assets);
        }

        public void buy(IAsset asset, int number)
        {
            double price = asset.Get_Price();
            if (price * number < cash)
            {
                cash -= price * number;
                Hedging_Portfolio.Add_Asset(asset, number);
            }
            else
            {
                throw new ArgumentOutOfRangeException("Not enough cash to buy asset(s)");
            }
        }

        public void sell(IAsset asset, int number)
        {
            double price = asset.Get_Price();
            cash += price * number;
            Hedging_Portfolio.Remove_Asset(asset, number);
        }

    }
}