using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class DataPoint
    {
        private DateTime date;
        private int value;
        public DataPoint(DateTime date, int value)
        {
            this.date = date;
            this.value = value;
        }

        public DateTime getDate()
        {
            return date;
        }

        public int getValue()
        {
            return value;
        }

        public string ToString()
        {
            // date is converted to timestamp (seconds starting 1/1/1970)
            return "[" + (long)(date.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000 + ", " + value + "]";
        }

    }
}