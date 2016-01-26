using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.DataBase
{
    public class CacheDB
    {
        private Dictionary<Tuple<string, DateTime>, double> _dic;
        private LinkedList<Tuple<string, DateTime>> _list;
        private uint _size;

        public CacheDB(uint size)
        {
            _size = size;
            _dic = new Dictionary<Tuple<string, DateTime>, double>();
            _list = new LinkedList<Tuple<string, DateTime>>();
        }

        public bool contains(string asset, DateTime date)
        {
            Tuple<string, DateTime> item = new Tuple<string, DateTime>(asset, date);
            return _dic.ContainsKey(item);
        }

        public double getPrice(string asset, DateTime date)
        {
            Tuple<string, DateTime> item = new Tuple<string, DateTime>(asset, date);
            if (_dic.ContainsKey(item))
            {
                return _dic[item];
            }
            else
            {
                throw new NoDataException(asset, date);
            }
        }

        public void setPrice(string asset, DateTime date, double price)
        {
            Tuple<string, DateTime> item = new Tuple<string, DateTime>(asset, date);
            _list.AddFirst(item);
            _dic[item] = price;
            if (_list.Count > _size)
            {
                item = _list.Last();
                _list.RemoveLast();
                _dic.Remove(item);
            }
        }

    }
}