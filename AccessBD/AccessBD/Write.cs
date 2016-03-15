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
                //List<KeyValuePair<int, DateTime>> list_pair_db = Access.getAllPricesKey(context);

                Price price_everglades;

                // si la date existe déjà dans la table des prix on la remplace
                if (Access.ContainsPricesKey(context, id, date))//list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    // on vérifie que la valeur de prix est différente
                    var priceBD = Access.Get_Price(id, date);
                    // si identiques on return (rien à faire)
                    if (price == priceBD["price"] && price == priceBD["priceEur"])
                    {
                        return;
                    }
                    // sinon on remplace
                    //price_everglades = Access.Get_PriceDB(id, date);
                    Access.Clear_Everglades_Price(date);
                    price_everglades = new Price { AssetDBId = id, date = date, price = price, priceEur = price };
                    context.Prices.Add(price_everglades);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    // sinon on l'ajoute
                    Price p = new Price { AssetDBId = id, date = date, price = price, priceEur =price };
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
                //List<DateTime> list_dates_db = Access.getAllKeysHedgingPortfolio(context);
                if (Access.ContainsHedgPortKey(context, date))
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


        public static void storePortfolioComposition(DateTime date, int assetId, double quantity)
        {
            using (var context = new qpcptfaw())
            {
                // si la date existe déjà dans la table des prix on la remplace
                if (Access.ContainsPortCompositionsKey(context, assetId, date))//list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    PortfolioComposition pc;
                    // on vérifie que la valeur de prix est différente
                    double quant = Access.getPortfolioComposition(assetId, date);
                    // si identiques on return (rien à faire)
                    if (quant==quantity)
                    {
                        return;
                    }
                    // sinon on remplace
                    //price_everglades = Access.Get_PriceDB(id, date);
                    Access.Clear_Portfolio_Composition(date, assetId);
                    pc = new PortfolioComposition { AssetDBId = assetId, date = date, quantity = quantity};
                    context.PortCompositions.Add(pc);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    // sinon on l'ajoute
                    PortfolioComposition p = new PortfolioComposition { AssetDBId = assetId, date = date, quantity = quantity };
                    context.PortCompositions.Add(p);
                    context.SaveChanges();
                    return;
                }
            }
        }

        public static void storeCholeskyMat(DateTime date, double[][] mat)
        {
            using (var context = new qpcptfaw())
            {
                // si la date existe déjà dans la table des prix on la remplace
                if (Access.ContainsCorrelKey(context, date))//list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    var mats = from m in context.CorrelVol
                               where m.date == date
                               select m;
                    mats.First().matrix = mat;
                    context.SaveChanges();
                    return;
                }
                else
                {
                    // sinon on l'ajoute
                    //PortfolioComposition p = new PortfolioComposition { AssetDBId = assetId, date = date, quantity = quantity };
                    CorrelDB data = new CorrelDB { date = date, matrix = mat };
                    context.CorrelVol.Add(data);
                    context.SaveChanges();
                    return;
                }
            }
        }

            public static void storeVolVect(DateTime date, double[] vol)
        {
            using (var context = new qpcptfaw())
            {
                // si la date existe déjà dans la table des prix on la remplace
                if (Access.ContainsCorrelKey(context, date))//list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    var mats = from m in context.CorrelVol
                               where m.date == date
                               select m;
                    mats.First().vol = vol;
                    context.SaveChanges();
                    return;
                }
                else
                {
                    // sinon on l'ajoute
                    //PortfolioComposition p = new PortfolioComposition { AssetDBId = assetId, date = date, quantity = quantity };
                    CorrelDB data = new CorrelDB { date = date, vol = vol };
                    context.CorrelVol.Add(data);
                    context.SaveChanges();
                    return;
                }
            }

        }



    }
}
