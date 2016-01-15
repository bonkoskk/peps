using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public class AmericanPut : IDerivative
    {
        public string getType()
        {
            return "American Put";
        }

        public List<Param> getParam()
        {
            throw new NotImplementedException();
        }

        public void setParam(List<Param> param)
        {
            throw new NotImplementedException();
        }

        public string getName()
        {
            throw new NotImplementedException();
        }

        public double getPrice()
        {
            throw new NotImplementedException();
        }

        public Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public double getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}