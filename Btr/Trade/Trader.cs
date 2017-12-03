using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Btr.Annotations;
using Btr.PrivApi;

namespace Btr
{
    public enum TradeMode { Wait, Buy, Sell }
    [DataContract]
    public class Treader : INotifyPropertyChanged
    {

        [DataMember]public CourseTracker Tracker { get; private set; }
        [DataMember] public Buyer Buyer { get; private set; }
        [DataMember] public List<Seller> Sellers { get; private set; }
        [DataMember] public List<Seller> Complited { get; private set; }
        [DataMember] public double KSellDist { get; set; }
        [DataMember] private bool _enabled;
        private bool _isBusy;
        [DataMember] private ApiParser _apiParser;
        private DateTime _lastTreaded;

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

        public Treader(CourseTracker tracker, ApiParser apiParser)
        {
            Tracker = tracker;
            _apiParser = apiParser;
            Buyer = new Buyer(apiParser);
            Complited = new List<Seller>();
            Sellers = new List<Seller>();
            Enabled = true;
            _isBusy = false;
        }


        private bool AllowBuy(CoursePoint pt)
        {
            if (!Sellers.Any()) return true;
            double minPrice = Sellers.Min(s=>s.BuyOrder.Price);
            double gap = KSellDist * Tracker.Sett.Delta * Math.Sqrt(Sellers.Count);
            return pt.Course < minPrice - gap;
        }

        private void TrySell(CoursePoint pt)
        {
            foreach(var seller in Sellers)
                seller.TrySell(pt, Tracker.GPrew);            
        }
        public async Task Trade(CoursePoint curCourse)
        {
            if (!Enabled || _isBusy) return;
            if (curCourse.Date < _lastTreaded + TradeMan.Interval) return;
            Debug.WriteLine("Trade {0}", _lastTreaded);
            _lastTreaded = curCourse.Date;
            _isBusy = true;
            await CheckComplOrders();
            var trackResult = Tracker.Track(curCourse);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("Mode={0} Pt{1}", Tracker.Leap.Mode, trackResult);
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
            if (await Buyer.IsCpmplited())
                Sellers.Add(new Seller(Buyer.PopComplited(), Tracker.Sett, _apiParser));
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
