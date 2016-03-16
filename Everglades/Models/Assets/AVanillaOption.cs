using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public abstract class AVanillaOption : IDerivative
    {
        protected IAsset underlying;
        protected double strike;
        protected DateTime maturity;
        protected double volatility;

        public List<Param> getParam()
        {
            List<Param> param = new List<Param>();
            param.Add(new Param("underlying", ParamType._equity));
            param.Add(new Param("strike", ParamType._double));
            param.Add(new Param("maturity", ParamType._date));
            return param;
        }

        public void setParam(List<Param> param)
        {
            Param[] P = param.ToArray();
            underlying = ModelManage.instance.Assets.Find(x => String.Compare(x.getName(), P[0].getString()) == 0);
            strike = P[1].getDouble();
            maturity = P[2].getDate();
        }

        public double getPrice()
        {
            return getPrice(DateTime.Now);
        }

        public Currency getCurrency()
        {
            return underlying.getCurrency();
        }

        public abstract string getName();
        public abstract double getDelta(DateTime t);
        public abstract double getPrice(DateTime t);
        public abstract Data getPrice(DateTime t1, DateTime t2, TimeSpan step);
        public abstract string getType();
        public abstract double getVolatility(DateTime t);



        public double getDividend(DateTime t1, DateTime t2)
        {
            return 0;
        }
    }
}