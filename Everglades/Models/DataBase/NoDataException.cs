using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public class NoDataException : Exception
    {
        private static string NoDataExceptionMessage(string asset, DateTime date)
        {
            return "No data in database for asset " + asset + " at date " + date;
        }

        public NoDataException(string asset, DateTime date)
            : base(NoDataExceptionMessage(asset, date))
        {
        }
    }
}