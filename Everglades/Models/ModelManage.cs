﻿using Everglades.Models.Assets;
using Everglades.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using AccessBD;
using Everglades.Models.Timers;

namespace Everglades.Models
{
    public class ModelManage
    {
        public static ModelManage instance = null;
        public static TimerList timers = new TimerList();

        public List<IAsset> Assets;
        public double cash;
        public uint shares_everg;
        public Everglades everg;
        public Portfolio Hedging_Portfolio;
        public LinkedList<Operation.Operation> Operations_History;
        public List<IDerivative> derivatives;

        // cache for last delta
        public DateTime today_date = DateTime.MinValue;
        public Portfolio today_delta;

        public ModelManage()
        {
            timers.start("ModelManage initialization");
            timers.start("Database initialization");
            qpcptfaw db = new qpcptfaw();
            //Access.Clear_Everglades_Prices();
            DBInitialisation.DBInit(db);
            timers.stop("Database initialization");
            /*
            for (int i = 1; i < 50 ; i++ )
            {
                DateTime d = DateTime.Today - TimeSpan.FromDays(i);
                AccessDB.setHedgingPortfolioValue(d, 0);
            }
            */

            //Access.ClearPrice(db, 65);
            //Access.ClearAsset(db, 65);
            //Access.ClearPrice(db, 68);
            //Access.ClearAsset(db, 68);
            //Access.ClearPrices(db);
            //Access.ClearDbConnections(db);
            //Access.ClearAssets(db);
            //Access.Clear_Everglades_Price(new DateTime(2016, 3, 2));
            try
            {
                Access.Clear_Everglades_Price(new DateTime(2016, 3, 9));
                Access.Clear_Everglades_Price(new DateTime(2016, 3, 10));
                Access.Clear_Everglades_Price(new DateTime(2016, 3, 11));
            }
            catch (Exception)
            {
                int balek = 1000000000;
            }



            instance = this;
            Assets = new List<IAsset>();
            List<ICurrency> Currenc = new List<ICurrency>();
            foreach (string name in AccessDB.Get_Asset_List())
            {
                Currencies curEnum = Access.GetEquityCurrencyFromSymbol(Access.GetSymbolFromName(name));
                Currency cur = new Currency(curEnum);
                Assets.Add(new Equity(name, cur));
                if (!Currenc.Any(x => x.getEnum() == curEnum) && curEnum != Currencies.EUR)
                {
                    Currenc.Add(cur);
                }
            }
            everg = new Everglades(Assets, Currenc);
            // TODO : cash should be in database
            shares_everg = 100;
            cash = shares_everg * everg.getPrice();
            Hedging_Portfolio = new Portfolio(Assets.Concat(Currenc.ConvertAll(x => (IAsset)x)).ToList());
            Operations_History = new LinkedList<Operation.Operation>();
            derivatives = new List<IDerivative>();
            derivatives.Add(new EuropeanCall());
            derivatives.Add(new EuropeanPut());
            derivatives.Add(new AmericanCall());
            derivatives.Add(new AmericanPut());
            derivatives.Add(new AsianCall());
            derivatives.Add(new AsianPut());
            timers.stop("ModelManage initialization");
        }

        public void buy(IAsset asset, int number)
        {
            double price = asset.getPrice();
            if (price * number < cash)
            {
                Hedging_Portfolio.addAsset(asset, number);
                cash -= price * number;
                Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "buy", asset, number, asset.getPrice()));
                AccessDB.setHedgingPortfolioValue(DateTime.Today, Hedging_Portfolio.getPrice());
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
            AccessDB.setHedgingPortfolioValue(DateTime.Today, Hedging_Portfolio.getPrice());
        }

        public List<Advice> getHedgingAdvice()
        {
            // getting or computing deltas of today
            if (today_date < DateTime.Today)
            {
                today_delta = everg.getDeltaPortfolio();
                today_date = DateTime.Today;
            }
            Portfolio deltas = today_delta;
            
            // create advices depending on current hedge
            List<Advice> list = new List<Advice>();
            foreach (KeyValuePair<IAsset, double> item in deltas.assetList)
            {
                string assetname = item.Key.getName();
                double difference = Hedging_Portfolio.assetList[item.Key] - item.Value * shares_everg;
                if (difference > 0.5)
                {
                    list.Add(new Advice(difference, assetname, "sell " + Convert.ToInt32(difference).ToString() + " of " + assetname));
                }
                else if (difference < - 0.5)
                {
                    list.Add(new Advice(-difference, assetname, "buy " + Convert.ToInt32(-difference).ToString() + " of " + assetname));
                }
            }
            return list;
        }

        public double getPnL() {
            return cash + Hedging_Portfolio.getPrice() - shares_everg * everg.getPrice();
        }

        public Data getHedgeForOne(DateTime t1, DateTime t2, TimeSpan step)
        {
            Data data = new Data();
            DateTime t = t1;
            while (t < t2)
            {
                //data.add(new DataPoint(t, ( Hedging_Portfolio.getPrice(t) + cash) / (double)shares_everg));
                try
                {
                    data.add(new DataPoint(t, (AccessDB.getPortfolioValue(t) / (double)shares_everg)));
                }
                catch (Exception)
                {

                }
                t += step;
            }
            try
            {
                data.add(new DataPoint(t2, AccessDB.getPortfolioValue(t2) / (double)shares_everg));
            }
            catch (Exception)
            {

            }
            return data;
        }

        /**
         * simulate an everglades product and it's underlying portfolio
         * evolution with the adviced hedging portfolio, and return a list
         * of Data :
         * * Data of product price evolution
         * * Data of hedging portfolio price evolution
         * * Data of tracking error evolution
         * * Data of cash spent for hedging portfolio evolution
         * 
         */
        public List<Data> simulateHedgeEvolution()
        {
            RandomNormal rand = new RandomNormal();
            LinkedList<DateTime> list_dates = everg.getObservationDates();
            LinkedList<DateTime> list_anticipated_dates = everg.getAnticipatedDates();
            DateTime first = list_dates.First();
            List<IAsset> simulated_list = new List<IAsset>();
            List<ICurrency> underlying_list_cur = new List<ICurrency>();
            foreach (IAsset ass in Assets)
            {
                simulated_list.Add(new AssetSimulated(ass, list_dates, rand));
                Currencies curEnum = ass.getCurrency().getEnum();
                if (!underlying_list_cur.Any(x => x.getEnum() == curEnum) && curEnum != Currencies.EUR)
                {
                    underlying_list_cur.Add(new CurrencySimulated(curEnum, rand));
                }
            }
            Everglades everg_simul = new Everglades(simulated_list, underlying_list_cur);
            Portfolio hedge_simul = new Portfolio(simulated_list);
            Data tracking_error = new Data();
            Data everglades_price = new Data();
            Data hedge_price = new Data();
            Data portsolo_price = new Data();
            Data cash_price = new Data();
            double cash_t = 0;
            double portvalue;
            double portsolovalue;
            double evergvalue;
            double r = 0.04;
            
            DateTime date_prev = list_dates.First();
            // used for anticipated end of everglades
            bool breakk = false;

            foreach (DateTime date in list_dates)
            {
                if (date == list_dates.First())
                {
                    evergvalue = everg_simul.computePrice(date).Item1;
                    hedge_simul = everg_simul.getDeltaPortfolio(date);
                    
                    portsolovalue = hedge_simul.getPrice(date);
                    cash_t = evergvalue - portsolovalue;
                    portvalue = portsolovalue + cash_t;
                }
                else
                {
                    // here we (virtually) sell old hedging portfolio
                    double t = (date - date_prev).TotalDays / 360;
                    portvalue = hedge_simul.getPrice(date) + cash_t * Math.Exp(r * t);
                    cash_t = portvalue;
                    // test if date is a constatation date
                    if (list_anticipated_dates.Contains(date))
                    {
                        // if the date is an anticipated constatation date, we check if
                        // we must break now, and if we do we set the price of everglades
                        // with payoff and set breakk to true.
                        Tuple<bool, double> payoff = everg_simul.getPayoff(date);
                        if (payoff.Item1)
                        {
                            evergvalue = payoff.Item2;
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= evergvalue;
                            breakk = true;
                        }
                        else
                        {
                            // if not the last date, we simply price the product and ajust our edge
                            evergvalue = everg_simul.computePrice(date).Item1;
                            hedge_simul = everg_simul.getDeltaPortfolio(date);
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= portsolovalue;
                        }
                    }
                    else if (date == everg_simul.getLastDate())
                    {
                        // if last date, we ge payoff and bam
                        Tuple<bool, double> payoff = everg_simul.getPayoff(date);
                        evergvalue = payoff.Item2;
                        cash_t -= evergvalue;
                        portsolovalue = hedge_simul.getPrice(date);
                    }
                    else
                    {
                        // if not the last date, we simply price the product and ajust our edge
                        evergvalue = everg_simul.computePrice(date).Item1;
                        hedge_simul = everg_simul.getDeltaPortfolio(date);
                        portsolovalue = hedge_simul.getPrice(date);
                        cash_t -= hedge_simul.getPrice(date);
                    }
                }
                
                double err = (evergvalue - portvalue) / evergvalue;
                
                if (!double.IsInfinity(evergvalue) && !double.IsNaN(evergvalue))
                {
                    everglades_price.add(new DataPoint(date, evergvalue));
                }
                if (!double.IsInfinity(portvalue) && !double.IsNaN(portvalue))
                {
                    hedge_price.add(new DataPoint(date, portvalue));
                }
                if (!double.IsInfinity(portsolovalue) && !double.IsNaN(portsolovalue))
                {
                    portsolo_price.add(new DataPoint(date, portsolovalue));
                }
                if (!double.IsInfinity(err) && !double.IsNaN(err))
                {
                    tracking_error.add(new DataPoint(date, err));
                }
                if (!double.IsInfinity(cash_t) && !double.IsNaN(cash_t))
                {
                    cash_price.add(new DataPoint(date, cash_t));
                }
                if (breakk)
                {
                    break;
                }
                date_prev = date;
            }
            List<Data> list = new List<Data>();
            list.Add(everglades_price);
            list.Add(hedge_price);
            list.Add(tracking_error);
            list.Add(cash_price);
            list.Add(portsolo_price);
            return list;
        }

    }
}