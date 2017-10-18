using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Lib;

namespace Btr
{
    public class TradeMan : List<Treader>
    {
        public static TimeSpan Interval{get { return new TimeSpan(0, 0, 3); }}
        public TradeMan()
        {
            _timer = new Timer(2000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            var from = DateTime.Now - MultiPeriodGrad.MaxPeriod;
            var period = new DatePeriod(from, DateTime.Now);
            Markets.LoadMarkets(period);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Markets.ReloadNew();
        }

        private Timer _timer;
        public void StartTrade()
        {
            _timer.Enabled = true;
            
        }
    }
}