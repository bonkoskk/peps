using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Param
    {
        private string name;
        private ParamType type;
        private int valueInt;
        private double valueDouble;
        private string valueString;
        private DateTime valueDate;

        public Param(string name, ParamType type)
        {
            this.name = name;
            this.type = type;
        }

        public string getName()
        {
            return name;
        }

        public ParamType getType()
        {
            return type;
        }

        public void setInt(int value)
        {
            this.valueInt = value;
        }

        public int getInt()
        {
            return valueInt;
        }

        public void setDouble(double value)
        {
            this.valueDouble = value;
        }

        public double getDouble()
        {
            return valueDouble;
        }

        public void setString(string value)
        {
            this.valueString = value;
        }

        public string getString()
        {
            return valueString;
        }

        public void setDate(DateTime date)
        {
            valueDate = date;
        }

        public DateTime getDate()
        {
            return valueDate;
        }
    }
}