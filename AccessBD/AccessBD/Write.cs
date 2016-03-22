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
                var ev = from e in context.Prices
                         where e.AssetDBId == id && e.date == date
                         select e;
                    // si la date existe déjà dans la table des prix on la remplace
                if (ev.Count()==1) //list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    // on vérifie que la valeur de prix est différente
                    // si identiques on return (rien à faire)
                    if (ev.First().price == price && ev.First().priceEur == price)
                    {
                        return;
                    }
                    // sinon on remplace
                    ev.First().price = price;
                    ev.First().priceEur = price;
                    context.SaveChanges();
                    return;
                }
                else if (ev.Count()==0)
                {
                    // sinon on l'ajoute
                    Price p = new Price { AssetDBId = id, date = date, price = price, priceEur =price };
                    context.Prices.Add(p);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    throw new Exception("Problème dans la BD.");
                }
            }
        }


        public static void storePortfolioValue(DateTime date, double value)
        {
            using (var context = new qpcptfaw())
            {

                var ev = from e in context.Portfolio
                         where e.date == date
                         select e;
                // si la date existe déjà dans la table des prix on la remplace
                if (ev.Count() == 1) //list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    // on vérifie que la valeur de prix est différente
                    // si identiques on return (rien à faire)
                    if (ev.First().value == value)
                    {
                        return;
                    }
                    // sinon on remplace
                    ev.First().value = value;
                    context.SaveChanges();
                    return;
                }
                else if (ev.Count() == 0)
                {
                    // sinon on l'ajoute
                    HedgingPortfolio p = new HedgingPortfolio { date = date, value =value };
                    context.Portfolio.Add(p);
                    context.SaveChanges();
                    return;
                }
                else
                {
                    throw new Exception("Problème dans la BD.");
                }
            }

        }


        public static void storePortfolioComposition(DateTime date, int assetId, double quantity)
        {
            using (var context = new qpcptfaw())
            {

                var ev = from e in context.PortCompositions
                         where e.date == date && e.AssetDBId == assetId
                         select e;
                // si la date existe déjà dans la table des prix on la remplace
                if (ev.Count() == 1) //list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                {
                    // on vérifie que la valeur de prix est différente
                    // si identiques on return (rien à faire)
                    if (ev.First().quantity == quantity)
                    {
                        return;
                    }
                    // sinon on remplace
                    ev.First().quantity = quantity;
                    context.SaveChanges();
                    return;
                }
                else if (ev.Count() == 0)
                {
                    // sinon on l'ajoute
                    PortfolioComposition p = new PortfolioComposition { AssetDBId = assetId, date = date, quantity = quantity };
                    context.PortCompositions.Add(p);
                    context.SaveChanges();
                    return;
                }else
                {
                    throw new Exception("Problème dans la BD.");
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

            public static void storeCashValue(DateTime date, double value)
            {
                using (var context = new qpcptfaw())
                {

                    var ev = from e in context.Cash
                             where e.date == date 
                             select e;
                    // si la date existe déjà dans la table des prix on la remplace
                    if (ev.Count() == 1) //list_pair_db.Contains(new KeyValuePair<int, DateTime>(id, date)))
                    {
                        // on vérifie que la valeur de prix est différente
                        // si identiques on return (rien à faire)
                        if (ev.First().value == value)
                        {
                            return;
                        }
                        // sinon on remplace
                        ev.First().value = value;
                        context.SaveChanges();
                        return;
                    }
                    else if (ev.Count() == 0)
                    {
                        CashDB c = new CashDB { date = date, value = value };
                        context.Cash.Add(c);
                        context.SaveChanges();
                        return;
                    }else
                    {
                        throw new Exception("Problème dans la BD.");
                    }
                }

            }



    }
}
