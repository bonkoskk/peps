using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccessBD;

namespace Everglades.Models.HistoricCompute
{
    public class RiskMetrics
    {
        public static double lambda = 0.94;
        public static int T = 30;
        public static DateTime t0 = new DateTime(2011,3,1);
        public static List<int> _list_id = new List<int>();

        public static List<DateTime> getWorkingDaysBefore(DateTime date, int days)
        {
            List<DateTime> list_dates = new List<DateTime>();
            int i = 0;
            DateTime date_local;
            while (list_dates.Count() < T)
            {
                date_local = date.AddDays(-i);
                if (date_local.DayOfWeek != DayOfWeek.Saturday && date_local.DayOfWeek != DayOfWeek.Sunday)
                {
                    list_dates.Add(date_local);
                }
                i+=1;
            }
            return list_dates;
        }

        public static double[][] var0()
        {
            if (_list_id.Count() == 0)
            {
                List<int> list_eq_id = Access.Get_List_Equities_id();
                List<int> list_curr_id = Access.Get_List_Forex_id();
                _list_id = list_eq_id.Concat(list_curr_id).ToList();
                _list_id.Sort();
            }
            int asset_nb = _list_id.Count();

            List<DateTime> list_dates = getWorkingDaysBefore(t0, T);

            double[][] var = new double[asset_nb][];

            list_dates.Sort();
            int dates_nb = list_dates.Count();

            Dictionary<DateTime, double>[] prices_mat = new Dictionary<DateTime, double>[asset_nb];
            Dictionary<DateTime, double>[] logret = new Dictionary<DateTime, double>[asset_nb];
            double[] means = new double[asset_nb];

            foreach (int id in _list_id)
            {
                prices_mat[id - 1] = Access.Get_PriceEur_RM(id, list_dates);
                logret[id - 1] = LogReturns.compute(prices_mat[id - 1], id);
                means[id-1] = ComputationTools.Mean(logret[id-1]);
                logret[id - 1] = ComputationTools.Diff(logret[id - 1], means[id - 1]);
            }            

            for (int i = 0; i < asset_nb; i++)
            {
                var[i] = new double[asset_nb];
                for (int j = 0; j < asset_nb; j++)
                {
                    var[i][j] = (1-lambda)*ComputationTools.WeightedSum(logret[i], logret[j], lambda);
                }
            }

            return var;
        }


        public static double[][] var(double[][] varprec, DateTime date)
        {
            if (_list_id.Count() == 0)
            {
                List<int> list_eq_id = Access.Get_List_Equities_id();
                List<int> list_curr_id = Access.Get_List_Forex_id();
                _list_id = list_eq_id.Concat(list_curr_id).ToList();
                _list_id.Sort();
            }
            int asset_nb = _list_id.Count();

            double[][] varcurr = new double[asset_nb][];

            Dictionary<DateTime, double>[] prices_mat = new Dictionary<DateTime, double>[asset_nb];
            Dictionary<DateTime, double>[] logret = new Dictionary<DateTime, double>[asset_nb];

            List<DateTime> list_dates = new List<DateTime>(){date.AddDays(-2), date.AddDays(-1)};

            foreach (int id in _list_id)
            {
                prices_mat[id - 1] = Access.Get_PriceEur_RM(id, list_dates);
                logret[id - 1] = LogReturns.compute(prices_mat[id - 1], id);
            }

            for (int i = 0; i < asset_nb; i++)
            {
                varcurr[i] = new double[asset_nb];
                for (int j = 0; j < asset_nb; j++)
                {
                    varcurr[i][j] = lambda * varprec[i][j] + (1 - lambda) * logret[i].First().Value * logret[j].First().Value;
                }
            }

            return varcurr;
        }

    

        public static void initRiskMetrics()
        {
            //double[,] var0 = HistoricCompute.RiskMetrics.var0();
                //double[,] var = HistoricCompute.RiskMetrics.var(var0, HistoricCompute.RiskMetrics.t0.AddDays(1));
            using(var context = new qpcptfaw()){
                if (context.CorrelVol.Count() == 0)
                {
                    double[][] varprec;
                    double[][] var;
                    double[][] var0 = HistoricCompute.RiskMetrics.var0();
                    varprec = var0;
                    //AccessBD.Write.storeCholeskyMat(RiskMetrics.t0, var0);
                    DateTime date = t0.AddDays(1);
                    while (date <= DateTime.Today)
                    {
                        if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                        {
                            var = HistoricCompute.RiskMetrics.var(varprec, date);
                            //AccessBD.Write.storeCholeskyMat(date, var);
                            varprec = var;
                        }
                        date = date.AddDays(1);
                    }
                }
                else
                {
                    double[][] varprec;
                    double[][] var;
                    Dictionary<DateTime, double[][]> dic = AccessBD.Access.GetLastVarMat();
                    varprec = dic.First().Value;
                    DateTime dateprec = dic.First().Key;
                    while (dateprec < DateTime.Today)
                    {
                        dateprec = dateprec.AddDays(1);
                        if (dateprec.DayOfWeek != DayOfWeek.Saturday && dateprec.DayOfWeek != DayOfWeek.Sunday)
                        {
                            var = HistoricCompute.RiskMetrics.var(varprec, dateprec);
                            //AccessBD.Write.storeCholeskyMat(dateprec, var);
                            varprec = var;
                        }
                    }
                }
            }
            
        }




    }
}