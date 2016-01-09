using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Operation
{
    public class Operation
    {
        private DateTime date;
        private string op;
        private IAsset asset;
        private int number;
        private double price;

        public Operation(DateTime date, string op, IAsset asset, int number, double price)
        {
            this.date = date;
            this.op = op;
            this.asset = asset;
            this.number = number;
            this.price = price;
        }

        public DateTime getDate()
        {
            return date;
        }

        public string getOperation()
        {
            return op; 
        }

        public IAsset getAsset()
        {
            return asset;
        }

        public int getNumber()
        {
            return number;
        }

        public double getPrice()
        {
            return number;
        }

    }
}