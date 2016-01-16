using Everglades.Models.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

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
                Assets.Add(new Equity(name, new Currency("$")));
            }
            everg = new Everglades(Assets);
            Hedging_Portfolio = new Portfolio(Assets);
            Operations_History = new LinkedList<Operation.Operation>();
            derivatives = new List<IDerivative>();
            derivatives.Add(new EuropeanCall());
            derivatives.Add(new EuropeanPut());
            derivatives.Add(new AmericanCall());
            derivatives.Add(new AmericanPut());
            derivatives.Add(new AsianCall());
            derivatives.Add(new AsianPut());


            simulateHedgeEvolution();
        }

        public void buy(IAsset asset, int number)
        {
            double price = asset.getPrice();
            if (price * number < cash)
            {
                Hedging_Portfolio.addAsset(asset, number);
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
            Hedging_Portfolio.removeAsset(asset, number);
            cash += price * number;
            Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "sell", asset, number, asset.getPrice()));
        }

        public List<Advice> getHedgingAdvice()
        {
            List<Advice> list = new List<Advice>();
            Portfolio deltas = everg.getDeltaPortfolio();
            int i = 0;
            foreach (KeyValuePair<IAsset, double> item in deltas.assetList)
            {
                string assetname = item.Key.getName();
                double difference = Hedging_Portfolio.assetList[item.Key] -  item.Value;
                if (difference > 0.01)
                {
                    list.Add(new Advice(difference, assetname, "sell " + difference.ToString() + " of " + assetname));
                }
                else if (difference < - 0.01)
                {
                    list.Add(new Advice(-difference, assetname, "buy " + (-difference).ToString() + " of " + assetname));
                }

            }
            return list;
        }

        public Data simulateHedgeEvolution()
        {
            RandomNormal rand = new RandomNormal();
            LinkedList<DateTime> list_dates = everg.getObservationDates();
            DateTime first = list_dates.First();

            List<IAsset> simulated_list = new List<IAsset>();
            foreach (IAsset ass in Assets)
            {
                simulated_list.Add(new AssetSimulated(ass, list_dates, rand));
            }
            Everglades everg_simul = new Everglades(simulated_list);

            Portfolio hedge_simul = new Portfolio(simulated_list);
            Data tracking_error = new Data();
            tracking_error.add(new DataPoint(first, 0));
            foreach (DateTime date in list_dates)
            {
                if (date == list_dates.First())
                {
                    continue;
                }
                double portvalue = everg_simul.getPrice(date);
                double err = (hedge_simul.getPrice(date) - portvalue) / portvalue;
                hedge_simul = everg_simul.getDeltaPortfolio(date);
                tracking_error.add(new DataPoint(date, err));
            }
            return tracking_error;
        }

    }
}