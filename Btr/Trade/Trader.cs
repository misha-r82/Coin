using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Btr.Log;
using Coin.Annotations;
using Coin.Files;
using Coin.Polon;

namespace Coin
{
    public enum TradeMode { Wait, Buy, Sell }
    [DataContract]
    public class Treader : INotifyPropertyChanged
    {

        [DataMember]public CourseTracker Tracker { get; private set; }
        [DataMember] public Buyer Buyer { get; private set; }
        [DataMember] public ObservableCollection<Seller> Sellers { get; private set; }
        [DataMember] public ObservableCollection<Seller> Complited { get; private set; }
        [DataMember] public double KSellDist { get; set; }
        [DataMember] public double MaxBuy { get; set; }
        [DataMember] public double MinSell { get; set; }
        [DataMember] private bool _enabled;
        private bool _isBusy;
        [DataMember] private Market _market;

        public Market Market
        {
            get { return _market; }
        }
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == _enabled) return;
                _enabled = value;
                OnPropertyChanged();
            }
        }

        public Treader()
        {
            Complited = new ObservableCollection<Seller>();
            Sellers = new ObservableCollection<Seller>();
            Enabled = true;
            _isBusy = false;
        }
        public Treader(Market market, TrackSettings trackSett) : this()
        {
            _market = market;
            Tracker = new CourseTracker(market, trackSett);
            Buyer = new Buyer(market.Api);
        }

        private bool AllowBuy(CoursePoint pt)
        {
            if (pt.Course > MaxBuy) return false;
            if (!Sellers.Any()) return true;
            double minPrice = Sellers.Min(s=>s.BuyOrder.Price);
            double gap = KSellDist * Tracker.Sett.Delta * Math.Sqrt(Sellers.Count);
            return pt.Course < minPrice - gap;
        }

        private void TrySell(CoursePoint pt)
        {
            if (pt.Course < MinSell) return;
            foreach(var seller in Sellers)
                seller.TrySell(pt);            
        }

        public async Task OnTick()
        {
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowTick))
                Log.CreateLog("Ontick", string.Format("Tick {0:HH:mm:ss}", DateTime.Now));
            if (!Enabled || _isBusy) return;
            if (Tracker.Market.LoadHistory())
                  Trade(Tracker.Market.LastPt);
        }

        public async Task Trade(CoursePoint curCourse)
        {
            _isBusy = true;
            await CheckComplOrders();
            var trackResult = Tracker.Track(curCourse);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Log.CreateLog("Trade", string.Format("Mode={0} Pt{1}", Tracker.Leap.Mode, trackResult));
            switch (trackResult)
            {
                case EndPoint.None:
                    if (Tracker.Leap.Mode == TrackMode.Neutral) TrySell(curCourse);
                    break;
                case EndPoint.UpEnd:
                    TrySell(curCourse); break;
                case EndPoint.DownEnd:
                    if (AllowBuy(curCourse))
                        Buyer.Buy(curCourse, Tracker); break;
            }
            _isBusy = false;
        }

        public async Task CheckComplOrders()
        {
            var deleted = new List<Seller>();
            foreach (Seller seller in Sellers)
            {
                if (await seller.IsCpmplited())
                {
                    deleted.Add(seller);
                    Complited.Add(seller);
                }
            }
            foreach (Seller seller in deleted)
                Sellers.Remove(seller);
            if (await Buyer.IsComplited())
            {
                var complitedOrder = Buyer.PopComplited();
                if (complitedOrder != null)
                    Sellers.Add(new Seller(complitedOrder, Tracker.Sett.Delta, _market.Api));
            }
        }
        [OnSerialized]
        public void OnSerialize(StreamingContext sc)
        {
            MarketSerializer.SerializeMarket(Market);
        }
        [OnDeserialized]
        public void OnDeserialized(StreamingContext sc)
        {
            var tmpMar = MarketSerializer.DeserializeMarket(Market);
            Market.CourseData = tmpMar.CourseData;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public struct CoursePoint
    {
        public double Course { get; }
        public DateTime Date { get; }

        public CoursePoint(double course, DateTime date)
        {
            Course = course;
            Date = date;
        }
        public override string ToString()
        {
            return string.Format("{0:dd.MM HH:mm}|{1:0.00000} ", Date, Course);
        }
    }
}
