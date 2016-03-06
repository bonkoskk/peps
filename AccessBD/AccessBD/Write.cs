using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessBD
{
    public class Write
    {
        public static void storeEvergladesPrice(DateTime date, double price)
        {
            using (var context = new qpcptfaw())
            {
                //récupère l'id d'Everglades
                int id = Access.GetIdEverglades();
                //récupère tous les clés id-date de la BD (table Prices)
                List<KeyValuePair<int, DateTime>> list_pair_db = Access.getAllPricesKey(context);

                Price price_everglades;

                //si la date existe déjà dans la table des prix
                if (list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    if (price == Access.Get_Price(id, date)["close"]) return;
                    price_everglades = Access.Get_PriceDB(id, date);
                    price_everglades.close = price;
                    price_everglades.open = price;
                    price_everglades.high = price;
                    price_everglades.low = price;
                    context.Prices.Add(price_everglades);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    Price p = new Price { AssetDBId = id, date = date, high = price, open = price, close = price, low = price, volume = 0 };
                    context.Prices.Add(p);
                    context.SaveChanges();
                    return;
                }
            }
        }


        public static void storePortfolioValue(DateTime date, double value)
        {
            using (var context = new qpcptfaw())
            {
                List<DateTime> list_dates_db = Access.getAllKeysHedgingPortfolio(context);
                if (list_dates_db.Contains(date))
                {
                    HedgingPortfolio hp = Access.getHedgingPortfolio(date);
                    if (value == hp.value) return;
                    hp.value = value;
                    context.Portfolio.Add(hp);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    HedgingPortfolio port = new HedgingPortfolio { date = date, value = value };
                    context.Portfolio.Add(port);
                    context.SaveChanges();
                    return;
                }
            }

        }


    }
}
