using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using Coin.Files;
using Lib;

namespace Coin
{
    [CollectionDataContract]
    [KnownType(typeof(Polon.ApiDriver))]
    public class TradeMan : List<Treader>, INotifyCollectionChanged
    {

        public static TimeSpan MinInterval { get { return new TimeSpan(7,0,0,0);} }
        public static TimeSpan Interval{get { return new TimeSpan(0, 5, 0); }}
        public static TimeSpan TickInterval { get { return new TimeSpan(0, 1, 0); } }
        [DataMember] private IApiDriver _apiDriver;
        [DataMember] public string DataDir { get; set; }
        public TradeMan()
        {
            _apiDriver = new Polon.ApiDriver();
            _timer = new Timer(TickInterval.TotalMilliseconds);
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
            foreach (var treader in this)
                treader.OnTick();
            _timer.Enabled = true;
        }

        private Timer _timer;
        public void StartTrade()
        {
            TimerOnElapsed(null, null);
        }

        public void StopTreading()
        {
            _timer.Enabled = false;
        }
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public Treader this[string platform, string market]
        {
            get { return this.First(t => t.Tracker.Market.Api.Name == platform && t.Market.Name == market); }
        }
//        [OnDeserializing]
//        private void OnDeserializing(StreamingContext c)
//        {
//            LoadMarkets();
//        }
    }
}