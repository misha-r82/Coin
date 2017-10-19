using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;
using Lib;

namespace Btr
{
    public class TradeMan : List<Treader>
    {
        public static TimeSpan Interval{get { return new TimeSpan(0, 5, 0); }}
        public TradeMan()
        {
            _timer = new Timer(3000);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            var from = DateTime.Now - new TimeSpan(0, 8, 0, 0);// MultiPeriodGrad.MaxPeriod;
            var period = new DatePeriod(from, DateTime.Now);
            Markets.LoadMarkets(period);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _timer.Enabled = false;
            Markets.ReloadNew();
            _timer.Enabled = true;
        }

        private Timer _timer;
        public void StartTrade()
        {
            _timer.Enabled = true;
            
        }
    }
}