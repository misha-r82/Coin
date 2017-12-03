using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Timers;
using Btr.Polon;
using Btr.PrivApi;
using Lib;

namespace Btr
{
    [CollectionDataContract]
    public class TradeMan : List<Treader>, INotifyCollectionChanged
    {
        
        public static TimeSpan Interval{get { return new TimeSpan(0, 5, 0); }}
        private static TimeSpan _tickInterval { get { return new TimeSpan(0, 0, 20); } }
        [DataMember] private ApiParser _apiParser;
        public TradeMan()
        {
            _apiParser = new ApiParser(new ApiBase());
            _timer = new Timer(_tickInterval.TotalMilliseconds);
            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = true;
            var from = DateTime.Now - MultiPeriodGrad.MaxPeriod;
            var period = new DatePeriod(from, DateTime.Now);
            //Markets.LoadMarkets(period);
        }
        public void Add(Treader treader)
        {
            base.Add(treader);
            OnCollectionChanged();
        }
        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            
            _timer.Enabled = false;
            //Debug.WriteLine("Timer {0}", DateTime.Now);
            DateTime loadedDate = Markets.MarketList.Values.Min(m => m.LastPt.Date);
            if (loadedDate <= DateTime.Now - Interval) // т.к. время соотв началу интервала
            {
                Markets.ReloadNew();
                loadedDate = Markets.MarketList.Values.Min(m => m.LastPt.Date);
                Debug.WriteLine("Loaded {0}", loadedDate);
            }
            foreach (var treader in this)
                treader.Trade(treader.Tracker.Market.LastPt);
            _timer.Enabled = true;
        }

        private Timer _timer;
        public void StartTrade()
        {
            _timer.Enabled = true;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        /*[OnDeserializing]
        private void OnDeserializing(StreamingContext c)
        {
            OnCreated();
        }*/
    }
}