﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Assets
{
    public class AsianCall : AVanillaOption
    {
        public override string getType()
        {
            return "Asian Call";
        }

        public override string getName()
        {
            throw new NotImplementedException();
        }

        public override Data getPrice(DateTime t1, DateTime t2, TimeSpan step)
        {
            throw new NotImplementedException();
        }

        public override double getPrice(DateTime t)
        {
            throw new NotImplementedException();
        }

        public override double getVolatility(DateTime t)
        {
            throw new NotImplementedException();
        }

        public override double getDelta(DateTime t)
        {
            throw new NotImplementedException();
        }
    }
}