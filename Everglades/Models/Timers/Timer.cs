using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models.Timers
{
    public class Timer
    {
        private string _name;
        private TimerStatus _status;
        private DateTime _start;
        private double _time; // in seconds

        public Timer(string name)
        {
            _name = name;
            _status = TimerStatus.Initialized;
        }

        public void start()
        {
            _start = DateTime.Now;
            _status = TimerStatus.Started;
        }

        public double stop()
        {
            _time = (DateTime.Now - _start).TotalSeconds;
            _status = TimerStatus.Stopped;
            return _time;
        }

        public override String ToString()
        {
            return _name + " : " + _time;
        }
    }
}