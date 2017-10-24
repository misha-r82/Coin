using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Timers;
using Btr.Polon;
using Btr.PrivApi;
using Lib;

namespace Btr
{
    [DataContract]
    public class TradeMan : List<Treader>
    {
        public static TimeSpan Interval{get { return new TimeSpan(0, 5, 0); }}
        private ApiParser _apiParser;
        public TradeMan()
        {
            _apiParser = new ApiParser(new ApiBase());
            _timer = new Timer(Interval.TotalMilliseconds);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            var from = DateTime.Now - new TimeSpan()//MultiPeriodGrad.MaxPeriod;
            var period = new DatePeriod(from, DateTime.Now);
            Markets.LoadMarkets(period);
        }

        public void Add(Market market, BaseSettings sett)
        {
            var tracker = new CourseTracker(market, sett);
            var treader = new Treader(tracker, _apiParser);
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