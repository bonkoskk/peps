using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Operation
{
    public class Operation
    {
        private string op;
        private IAsset asset;
        private int number;

        public Operation(string op, IAsset asset, int number)
        {
            this.op = op;
            this.asset = asset;
            this.number = number;
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

    }
}