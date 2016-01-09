﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class Data
    {
        private List<DataPoint> data;

        public Data()
        {
            data = new List<DataPoint>();
        }

        public void add(DataPoint point)
        {
            data.Add(point);
        }

        public override string ToString()
        {
            string s = "[";
            bool first = true;
            foreach (DataPoint point in data)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    s += ", ";
                }
                s += point.ToString();
            }
            s += "]";
            return s;
        }

    }
}