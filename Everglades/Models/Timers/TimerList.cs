using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Timers
{
    public class TimerList
    {
        public Dictionary<string, Timer> dic;
        public TimerList()
        {
            dic = new Dictionary<string, Timer>();
        }

        public void start(string name)
        {
            if (dic.ContainsKey(name))
            {
                dic[name].start();
            }
            else
            {
                Timer t = new Timer(name);
                dic[name] = t;
                t.start();
            }
        }

        // return time since started in seconds
        public double stop(string name)
        {
            if (!dic.ContainsKey(name))
            {
                throw new ArgumentOutOfRangeException("unknown name");
            }
            else
            {
                return dic[name].stop().TotalSeconds;
            }
        }

    }
}