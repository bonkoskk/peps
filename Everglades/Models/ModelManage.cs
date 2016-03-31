using Everglades.Models.Assets;
using Everglades.Models.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using AccessBD;
using Everglades.Models.Timers;
using System.Xml;

namespace Everglades.Models
{
    public class ModelManage
    {
        public static ModelManage instance = null;
        public static TimerList timers = new TimerList();

        public List<IAsset> Assets;
        public List<ICurrency> Assets_Currencies;
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
            //double[,] var0 = HistoricCompute.RiskMetrics.var0();
            //double[,] var = HistoricCompute.RiskMetrics.var(var0, HistoricCompute.RiskMetrics.t0.AddDays(1));
            timers.start("ModelManage initialization");
            timers.start("Database initialization");
            qpcptfaw db = new qpcptfaw();
            //Access.Clear_Everglades_Prices();


            DBInitialisation.DBInit(db);
            HistoricCompute.RiskMetrics.initRiskMetrics(24);
            
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
                //Access.Clear_Everglades_Price(new DateTime(2016, 3, 9));
                //Access.Clear_Everglades_Price(new DateTime(2016, 3, 10));
                //Access.Clear_Everglades_Price(new DateTime(2016, 3, 11));
                //Access.Clear_Everglades_Price(new DateTime(2016, 3, 5));
                //Access.Clear_Prices_After(new DateTime(2016, 2, 1), Access.GetIdEverglades());
            }
            catch (Exception)
            {}

            // initialize main variables and lists
            instance = this;
            Assets = new List<IAsset>();
            Assets_Currencies = new List<ICurrency>();
            Dictionary<string, Currencies> curEnum = Access.Get_Equities_Currencies();
            foreach (KeyValuePair<string, Currencies> ent in curEnum)
            {
                Currency cur = new Currency(ent.Value);
                Assets.Add(new Equity(ent.Key, cur));
                if (!Assets_Currencies.Any(x => x.getEnum() == ent.Value) && ent.Value != Currencies.EUR)
                {
                    Assets_Currencies.Add(cur);
                }
            }
            everg = new Everglades(Assets, Assets_Currencies);
            shares_everg = 100000;
            // TODO : change or delete



            //simulateBackTestEvolution(true, DateTime.Today - TimeSpan.FromDays(10), TimeSpan.FromDays(1));


            
            try
            {
                cash = Access.getCashDB(DateTime.Today).value;
                Hedging_Portfolio = getHedgingPortfolioFromBD(DateTime.Today);
            }
            catch (Exception)
            {
                cash = shares_everg * everg.getPrice();
                Hedging_Portfolio = new Portfolio(Assets.Concat(Assets_Currencies.ConvertAll(x => (IAsset)x)).ToList());
            }
            DateTime d_temp = DateTime.Today;
            //Hedging_Portfolio = new Portfolio(Assets.Concat(Assets_Currencies.ConvertAll(x => (IAsset)x)).ToList());
            
            
            Operations_History = new LinkedList<Operation.Operation>();
            derivatives = new List<IDerivative>();
            derivatives.Add(new EuropeanCall());
            derivatives.Add(new EuropeanPut());
            derivatives.Add(new AmericanCall());
            derivatives.Add(new AmericanPut());
            derivatives.Add(new AsianCall());
            derivatives.Add(new AsianPut());
            derivatives.Add(new QuantoCall());
            derivatives.Add(new QuantoPut());
            timers.stop("ModelManage initialization");
        }

        public void buy(IAsset asset, int number)
        {
            double price = asset.getPriceEuro(DateTime.Today);
            if (price * number < cash)
            {
                // manage C# portfolio
                Hedging_Portfolio.addAsset(asset, number);
                cash -= price * number;
                Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "buy", asset, number, asset.getPrice()));
                // manage BD portfolio
                if (asset is Equity) {
                    int asset_id = Access.GetIdFromName(asset.getName());
                    double total = Access.getPortfolioComposition(asset_id, DateTime.Today);
                    AccessBD.Write.storePortfolioComposition(DateTime.Today, asset_id, total + number);
                    AccessDB.setHedgingPortfolioValue(DateTime.Today, (Hedging_Portfolio.getPrice() + cash));
                    Write.storeCashValue(DateTime.Today, cash);
                }
                else if (asset is Currency)
                {
                    int cur_id = Access.getForexIdFromCurrency(((Currency)asset).getEnum());
                    double total = Access.getPortfolioComposition(cur_id, DateTime.Today);
                    Write.storePortfolioComposition(DateTime.Today, cur_id, total + number);
                    AccessDB.setHedgingPortfolioValue(DateTime.Today, Hedging_Portfolio.getPrice() + cash);
                    Write.storeCashValue(DateTime.Today, cash);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Not enough cash to buy asset(s)");
            }
        }

        public void sell(IAsset asset, int number)
        {
            // manage C# portfolio
            double price = asset.getPrice();
            Hedging_Portfolio.removeAsset(asset, number);
            cash += price * number;
            Operations_History.AddFirst(new Operation.Operation(DateTime.Now, "sell", asset, number, asset.getPrice()));
            // manage BD portfolio
            if (asset is Equity)
            {
                int asset_id = Access.GetIdFromName(asset.getName());
                double total = Access.getPortfolioComposition(asset_id, DateTime.Today);
                AccessBD.Write.storePortfolioComposition(DateTime.Today, asset_id, total - number);
                AccessDB.setHedgingPortfolioValue(DateTime.Today, (Hedging_Portfolio.getPrice() + cash));
                Write.storeCashValue(DateTime.Today, cash);
            }
            else if (asset is Currency)
            {
                int cur_id = Access.getForexIdFromCurrency(((Currency)asset).getEnum());
                double total = Access.getPortfolioComposition(cur_id, DateTime.Today);
                Write.storePortfolioComposition(DateTime.Today, cur_id, total - number);
                AccessDB.setHedgingPortfolioValue(DateTime.Today, Hedging_Portfolio.getPrice() + cash);
                Write.storeCashValue(DateTime.Today, cash);
            }
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
                double difference = item.Value * shares_everg - Hedging_Portfolio.assetList[item.Key];
                if (difference > 0.5)
                {
                    list.Add(new Advice(difference / shares_everg, assetname, "buy " + Convert.ToInt32(difference).ToString() + " of " + assetname));
                }
                else if (difference < - 0.5)
                {
                    list.Add(new Advice(difference / shares_everg, assetname, "sell " + Convert.ToInt32(-difference).ToString() + " of " + assetname));
                }
            }
            return list;
        }

        public List<Advice> applyHedgingAdvice()
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
                IAsset asset = item.Key;
                double difference = item.Value * shares_everg - Hedging_Portfolio.assetList[item.Key];
                int difference_int = Convert.ToInt32(difference);
                if (difference > 0.5)
                {
                    buy(asset, difference_int);
                }
                else if (difference < -0.5)
                {
                    sell(asset, -difference_int);
                }
            }
            return list;
        }

        public double getPnL() {
            double evprice = everg.getPrice();
            return (cash + Hedging_Portfolio.getPrice() - shares_everg * evprice * 100.0) / (evprice * shares_everg);
        }

        public Data getHedgeForOne(DateTime t1, DateTime t2, TimeSpan step)
        {
            Data data = new Data("hedge");
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
        public List<Data> simulateHedgeEvolution(bool with_currency)
        {
            RandomNormal rand = new RandomNormal();
            LinkedList<DateTime> list_dates = everg.getObservationDates();
            LinkedList<DateTime> list_anticipated_dates = everg.getAnticipatedDates();
            DateTime first = list_dates.First();
            List<IAsset> simulated_list = new List<IAsset>();
            List<ICurrency> underlying_list_cur = new List<ICurrency>();
            double r = 0.03;
            foreach (IAsset ass in Assets)
            {
                simulated_list.Add(new AssetSimulated(ass, list_dates, rand));
                if (with_currency)
                {
                    Currencies curEnum = ass.getCurrency().getEnum();
                    if (!underlying_list_cur.Any(x => x.getEnum() == curEnum) && curEnum != Currencies.EUR)
                    {
                        underlying_list_cur.Add(new CurrencySimulated(curEnum, rand, r));
                    }
                }
            }
            Everglades everg_simul = new Everglades(simulated_list, underlying_list_cur);
            Portfolio hedge_simul = new Portfolio(simulated_list);
            Data tracking_error = new Data("simulation-graph-trackingerror");
            Data everglades_price = new Data("simulation-graph-prices-everg");
            Data hedge_price = new Data("simulation-graph-prices-hedge");
            Data portsolo_price = new Data("simulation-graph-soloport");
            Data cash_price = new Data("simulation-graph-cash");
            Dictionary<String, Data> list_asset_price = new Dictionary<string, Data>();
            foreach (IAsset asset in simulated_list)
            {
                String nameTemp = asset.getName();
                list_asset_price[nameTemp] = new Data(nameTemp);
            }
            foreach (IAsset cur in underlying_list_cur)
            {
                String nameTemp = cur.getName();
                list_asset_price[nameTemp] = new Data(nameTemp);
            }

            double cash_t = 0;
            double portvalue;
            double portsolovalue;
            double evergvalue;
            
            DateTime date_prev = list_dates.First();
            // used for anticipated end of everglades
            bool breakk = false;
            foreach (DateTime date in list_dates)
            {
                // get prices of assets at these dates in Data
                foreach (IAsset asset in simulated_list)
                {
                    String nameTemp = asset.getName();
                    list_asset_price[nameTemp].add(new DataPoint(date, asset.getPrice(date)));
                }
                foreach (IAsset cur in underlying_list_cur)
                {
                    String nameTemp = cur.getName();
                    list_asset_price[nameTemp].add(new DataPoint(date, cur.getPrice(date)));
                }


                if (date == list_dates.First())
                {
                    Tuple<double, double[], bool> compute = everg_simul.computePrice(date, with_currency);
                    evergvalue = compute.Item1;
                    hedge_simul = everg_simul.getDeltaPortfolio(date, compute.Item2, with_currency);
                    
                    portsolovalue = hedge_simul.getPrice(date);
                    cash_t = evergvalue - portsolovalue;
                    portvalue = portsolovalue + cash_t;
                }
                else
                {
                    // here we (virtually) sell old hedging portfolio
                    double t = (date - date_prev).TotalDays / 360;
                    portvalue = hedge_simul.getPrice(date) + hedge_simul.getDividend(date_prev, date) + cash_t * Math.Exp(r * t);
                    cash_t = portvalue;
                    Tuple<double, double[], bool> compute = everg_simul.computePrice(date, with_currency);
                    // test if date is a constatation date
                    if (list_anticipated_dates.Contains(date))
                    {
                        // if the date is an anticipated constatation date, we check if
                        // we must break now, and if we do we set the price of everglades
                        // with payoff and set breakk to true.
                        if (compute.Item3)
                        {
                            evergvalue = compute.Item1;
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= evergvalue;
                            breakk = true;
                        }
                        else
                        {
                            // if not the last date, we simply price the product and ajust our edge
                            evergvalue = compute.Item1;
                            hedge_simul = everg_simul.getDeltaPortfolio(date, compute.Item2, with_currency);
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= portsolovalue;
                        }
                    }
                    else if (date == everg_simul.getLastDate())
                    {
                        evergvalue = compute.Item1;
                        cash_t -= evergvalue;
                        portsolovalue = hedge_simul.getPrice(date);
                    }
                    else
                    {
                        // if not the last date, we simply price the product and ajust our edge
                        evergvalue = compute.Item1;
                        hedge_simul = everg_simul.getDeltaPortfolio(date, compute.Item2, with_currency);
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
            foreach (Data dat in list_asset_price.Values)
            {
                list.Add(dat);
            }
            return list;
        }

        /**
         * recalculate everglades price and hedging portfolio on the firstDate -> Today period
         * with a date every step, with currency management if with_currency is true
         */
        public void simulateBackTestEvolution(bool with_currency, DateTime firstDate, DateTime DateEnd, TimeSpan step, bool recover = false)
        {
            firstDate = new DateTime(firstDate.Year, firstDate.Month, firstDate.Day);
            RandomNormal rand = new RandomNormal();
            DateTime current_date;
            // clean data from firstDate to today
            current_date = firstDate;
            if (!recover)
            {
                while (current_date <= DateEnd)
                {
                    try
                    {
                        Access.Clear_Everglades_Price(current_date);
                    }
                    catch (ArgumentException) { }
                    try
                    {
                        Access.Clear_Portfolio_Price(current_date);
                    }
                    catch (ArgumentException) { }
                    current_date = current_date + TimeSpan.FromDays(1);
                }
            }
            // create list of date for calculus            
            current_date = firstDate;
            LinkedList<DateTime> list_dates = new LinkedList<DateTime>();
            while (current_date <= DateEnd)
            {
                list_dates.AddLast(current_date);
                current_date = current_date + step;
            }
            LinkedList<DateTime> list_anticipated_dates = everg.getAnticipatedDates();
            Portfolio hedge_simul = null;

            double cash_t = 0;
            double portvalue;
            double portsolovalue;
            double evergvalue;

            DateTime date_prev = list_dates.First();
            // used for anticipated end of everglades
            bool breakk = false;
            foreach (DateTime date in list_dates)
            {
                double r = everg.getCurrency().getInterestRate(date_prev);
                if (date == list_dates.First())
                {
                    Tuple<double, double[], bool> compute = everg.computePrice(date, with_currency);
                    evergvalue = compute.Item1;
                    hedge_simul = everg.getDeltaPortfolio(date, compute.Item2, with_currency);
                    portsolovalue = hedge_simul.getPrice(date);
                    if (recover)
                    {
                        cash_t = AccessDB.getPortfolioValue(firstDate) / (double)shares_everg - portsolovalue;
                    }
                    else
                    {
                        cash_t = evergvalue - portsolovalue;
                    }
                    portvalue = portsolovalue + cash_t;
                }
                else
                {
                    // here we (virtually) sell old hedging portfolio
                    double t = (date - date_prev).TotalDays / 360;
                    portvalue = hedge_simul.getPrice(date) + hedge_simul.getDividend(date_prev, date) + cash_t * Math.Exp(r * t);
                    cash_t = portvalue;
                    Tuple<double, double[], bool> compute = everg.computePrice(date, with_currency);
                    // test if date is a constatation date
                    if (list_anticipated_dates.Contains(date))
                    {
                        // if the date is an anticipated constatation date, we check if
                        // we must break now, and if we do we set the price of everglades
                        // with payoff and set breakk to true.
                        if (compute.Item3)
                        {
                            evergvalue = compute.Item1;
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= evergvalue;
                            breakk = true;
                        }
                        else
                        {
                            // if not the last date, we simply price the product and ajust our edge
                            evergvalue = compute.Item1;
                            hedge_simul = everg.getDeltaPortfolio(date, compute.Item2, with_currency);
                            portsolovalue = hedge_simul.getPrice(date);
                            cash_t -= portsolovalue;
                        }
                    }
                    else if (date == everg.getLastDate())
                    {
                        // if last date, we ge payoff and bam
                        //Tuple<bool, double> payoff = everg_simul.getPayoff(date);
                        evergvalue = compute.Item1; //payoff.Item2;
                        cash_t -= evergvalue;
                        portsolovalue = hedge_simul.getPrice(date);
                    }
                    else
                    {
                        // if not the last date, we simply price the product and ajust our edge
                        evergvalue = compute.Item1;
                        hedge_simul = everg.getDeltaPortfolio(date, compute.Item2, with_currency);
                        portsolovalue = hedge_simul.getPrice(date);
                        cash_t -= hedge_simul.getPrice(date);
                    }
                }

                double err = (evergvalue - portvalue) / evergvalue;

                if (!double.IsInfinity(evergvalue) && !double.IsNaN(evergvalue))
                {
                    Write.storeEvergladesPrice(date, evergvalue);
                }
                if (!double.IsInfinity(portvalue) && !double.IsNaN(portvalue))
                {
                    Write.storePortfolioValue(date, portvalue * shares_everg);
                }
                if (breakk)
                {
                    break;
                }
                date_prev = date;
            }
            // save last result in BD
            foreach (IAsset asset in hedge_simul.assetList.Keys) {
                if (asset is Equity)
                {
                    int asset_id = Access.GetIdFromName(asset.getName());
                    double quantity = hedge_simul.assetList[asset];
                    Write.storePortfolioComposition(DateEnd, asset_id, Math.Round(quantity * shares_everg));
                } 
                else if (asset is Currency)
                {
                    int cur_id = Access.getForexIdFromCurrency(((Currency)asset).getEnum()); ;
                    double quantity = hedge_simul.assetList[asset];
                    Write.storePortfolioComposition(DateEnd, cur_id, Math.Round(quantity * shares_everg));
                }
            }
            Write.storeCashValue(DateEnd, cash_t * shares_everg);
        }


        //retourne la composition d'un portefeuille stocké en BD à une date t
        private Portfolio getHedgingPortfolioFromBD(DateTime t)
        {
            Dictionary<int, double> portcomp;
            Portfolio port = new Portfolio(Assets.Concat(Assets_Currencies.ConvertAll(x => (IAsset)x)).ToList());
            //tente de récupérer le portefeuille stocké en BD
            try
            {
                //s'il y a un portefeuille en BD, on récupère ses assets et quantités
                portcomp = Access.getHedgingPortfolioTotalComposition(t);
                //on ajoute la quantité d'asset
                foreach (IAsset ass in Assets)
                {
                    int id = Access.GetIdFromName(ass.getName());
                    if (portcomp.ContainsKey(id))
                    {
                        port.addAsset(ass, portcomp[id]);
                    }
                }
                //on ajoute la quantité de currencies
                foreach (ICurrency ass in Assets_Currencies)
                {
                    int id = Access.getForexIdFromCurrency(ass.getEnum());
                    if (portcomp.ContainsKey(id))
                    {
                        port.addAsset(ass, portcomp[id]);
                    }
                }
                return port;
            }
            catch (ArgumentException)
            {
                //s'il n'y a pas de portefeuille en BD, on retourne le portefeuille vide
                return port;
            }
        }


        public String hedgingPortfolioComposition()
        {
            string str = "[";
            double equ = 0.0, vanilla = 0.0, exo = 0.0, cur = 0.0;
            foreach (KeyValuePair<IAsset, double> item in Hedging_Portfolio.assetList)
            {
                if (item.Value != 0)
                {
                    double price = item.Value * item.Key.getPrice();
                    if (price < 0)
                    {
                        price = -price;
                    }
                    if (item.Key is Equity)
                    {
                        equ += price;
                    }
                    else if (item.Key is AVanillaOption)
                    {
                        vanilla += price;
                    }
                    else if (item.Key is IDerivative)
                    {
                        exo += price;
                    }
                    else if (item.Key is ICurrency)
                    {
                        cur += price;
                    }
                }
            }

            if (this.cash == 0 && (Hedging_Portfolio.assetList.Keys.Count == 0 || (equ == 0 && vanilla == 0 && exo == 0)))
            {
                str += "{label: \"Empty\", data: 1}";
            }
            else
            {
                str += "{label: \"Cash\", data: " + XmlConvert.ToString(this.cash) + "},";
                str += "{label: \"Equities\", data: " + XmlConvert.ToString(equ) + "},";
                str += "{label: \"Vanilla Options\", data: " + XmlConvert.ToString(vanilla) + "},";
                str += "{label: \"Exotic Options\", data: " + XmlConvert.ToString(exo) + "},";
                str += "{label: \"Currencies\", data: " + XmlConvert.ToString(cur) + "}";
            }
            str += "]";
            return str;
        }


        protected struct DatCache
        {
            public DateTime t1;
            public DateTime t2;
            public TimeSpan step;
            public String str;
        };

        private DatCache cacheHedge = new DatCache { t1 = DateTime.MinValue, t2 = DateTime.MinValue, step = TimeSpan.MinValue, str = "" };

        public String getHedgeForOneString(DateTime t1, DateTime t2, TimeSpan step) {
            if (!(t1 == cacheHedge.t1 && t2 == cacheHedge.t2 && step == cacheHedge.step))
            {
                cacheHedge.t1 = t1;
                cacheHedge.t2 = t2;
                cacheHedge.step = step;
                cacheHedge.str = getHedgeForOne(t1, t2, step).ToString();
            }
            return cacheHedge.str;
        }

        private DatCache cacheEverg = new DatCache { t1 = DateTime.MinValue, t2 = DateTime.MinValue, step = TimeSpan.MinValue, str = "" };

        public String evergGetPriceString(DateTime t1, DateTime t2, TimeSpan step)
        {
            if (!(t1 == cacheEverg.t1 && t2 == cacheEverg.t2 && step == cacheEverg.step))
            {
                cacheEverg.t1 = t1;
                cacheEverg.t2 = t2;
                cacheEverg.step = step;
                cacheEverg.str = everg.getPrice(t1, t2, step).ToString();
            }
            return cacheEverg.str;
        }

    }
}