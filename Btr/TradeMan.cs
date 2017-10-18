using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace Btr
{
    public class TradeMan : List<Treader>
    {
        public static TimeSpan Interval{get { return new TimeSpan(0, 0, 3); }}
        public TradeMan()
        {
            _timer = new Timer(2000);
            _timer.Elapsed += (sender, args) =>
            {
                Debug.WriteLine(DateTime.Now);
                Console.WriteLine(DateTime.Now);
            };
            _timer.AutoReset = true;
        }
        private Timer _timer;
        public void StartTrade()
        {
            _timer.Enabled = true;
            
        }
    }
}