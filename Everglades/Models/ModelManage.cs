using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class ModelManage
    {
        public static ModelManage instance = null;

        public List<IAsset> Assets;
        public double cash;
        public Everglades everg;
        public Portfolio Hedging_Portfolio;
        public LinkedList<Operation.Operation> Operations_History;
        public List<IDerivative> derivatives;

        public ModelManage()
        {
            instance = this;
            cash = 10000;
            Assets = new List<IAsset>();
            foreach (string name in AccessDB.Get_Asset_List())
            {
                Assets.Add(new Equity(name));
            }
            everg = new Everglades();
            Hedging_Portfolio = new Portfolio(Assets);
            Operations_History = new LinkedList<Operation.Operation>();
            derivatives = new List<IDerivative>();
            derivatives.Add(new EuropeanCall());
            derivatives.Add(new EuropeanPut());
            derivatives.Add(new AmericanCall());
            derivatives.Add(new AmericanPut());
            derivatives.Add(new AsianCall());
            derivatives.Add(new AsianPut());
        }

        public void buy(IAsset asset, int number)
        {
            double price = asset.getPrice();
            if (price * number < cash)
            {
                Hedging_Portfolio.Add_Asset(asset, number);
                cash -= price * number;
                Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "buy", asset, number, asset.getPrice()));
            }
            else
            {
                throw new ArgumentOutOfRangeException("Not enough cash to buy asset(s)");
            }
        }

        public void sell(IAsset asset, int number)
        {
            double price = asset.getPrice();
            Hedging_Portfolio.Remove_Asset(asset, number);
            cash += price * number;
            Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "sell", asset, number, asset.getPrice()));
        }

    }
}