using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrapping;

namespace Everglades.Models.Assets
{
    public class EuropeanCall : IDerivative
    {
        private IAsset underlying;
        private double strike;
        private DateTime maturity;

        public EuropeanCall()
        {

        }

        string IDerivative.getType()
        {
            return "European Call";
        }

        string IAsset.getName()
        {
            return "European Call on asset " + underlying.getName();
        }

        List<Param> IDerivative.getParam()
        {
            List<Param> param = new List<Param>();
            param.Add(new Param("underlying", ParamType._equity));
            param.Add(new Param("strike", ParamType._double));
            param.Add(new Param("maturity", ParamType._date));
            return param;
        }

        void IDerivative.setParam(List<Param> param)
        {
            Param[] P = param.ToArray();
            underlying = ModelManage.instance.Assets.Find(x => String.Compare(x.getName(), P[0].getString()) == 0);
            strike = P[1].getDouble();
            maturity = P[2].getDate();
        }

        double IAsset.getPrice()
        {
            WrapperVanilla wc = new WrapperVanilla();
            double T = (maturity - DateTime.Now).TotalDays / 365;
            if (T < 0)
            {
                throw new ArgumentOutOfRangeException("Maturity must be in future");
            }
            wc.getPriceOptionEuropeanCall(T, underlying.getPrice(), strike, underlying.getVolatility(DateTime.Now), AccessDB.Get_Interest_Rate("euro", DateTime.Now), 0);
            double price = wc.getPrice();
            return wc.getPrice();
        }

        Data IAsset.getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        double IAsset.getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        double IAsset.getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }



        double IAsset.getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}