using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.HistoricCompute
{
    public class LogReturns
    {
        internal static Dictionary<DateTime, double> compute(Dictionary<DateTime, double> prices, int assetid)
        {
            bool accessbd = false;
            Dictionary<DateTime, double> returns = new Dictionary<DateTime, double>();
            DateTime first = prices.Min(t=>t.Key);
            DateTime before;
            double price = 1;
            foreach (KeyValuePair<DateTime, double> kvp in prices)
            {
                if(kvp.Key != first){
                    before = kvp.Key.AddDays(-1);
                    while (!prices.ContainsKey(before) && before >= first) before = before.AddDays(-1);
                    if (!prices.ContainsKey(before) && before < first)
                    {
                        while (1 == 1)
                        {
                            try
                            {
                                price = AccessBD.Access.get_Price_Eur(assetid, before);
                                accessbd = true;
                                break;
                            }
                            catch (ArgumentException)
                            {
                                before = before.AddDays(-1);
                            }
                        }
                        
                    }
                    if (accessbd == false)
                    {
                        price = prices[before];
                    }
                    returns[kvp.Key] = Math.Log(prices[kvp.Key] / price);
                }
            }
            return returns;
        }
    }
}