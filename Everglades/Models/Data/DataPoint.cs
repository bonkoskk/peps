using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class DataPoint
    {
        private DateTime date;
        private double value;
        public DataPoint(DateTime date, double value)
        {
            this.date = date;
            this.value = value;
        }

        public DateTime getDate()
        {
            return date;
        }

        public double getValue()
        {
            return value;
        }

        public override string ToString()
        {
            // date is converted to timestamp (seconds starting 1/1/1970)
            return "[" + (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000 + ", " + value + "]";
        }

    }
}