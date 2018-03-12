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

        public static TimeSpan MinInterval { get { return new TimeSpan(1,0,0,0);} }
        public static TimeSpan Interval{get { return new TimeSpan(0, 5, 0); }}
        public static TimeSpan TickInterval { get { return new TimeSpan(0, 0, 20); } }
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

        private void ReloadNew()
        {
            var actions = new List<Action>();
            foreach (var treader in this)
                actions.Add(() => treader.Tracker.Market.LoadHistory());
            var pOpt = new ParallelOptions() { MaxDegreeOfParallelism = 16 };
            Parallel.Invoke(pOpt, actions.ToArray());
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
            _timer.Enabled = true;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged()
        {
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
//        [OnDeserializing]
//        private void OnDeserializing(StreamingContext c)
//        {
//            LoadMarkets();
//        }
    }
}