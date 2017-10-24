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
        public CourseTracker Tracker { get; }
        private ApiParser _apiParser { get; }
        [DataMember]public Buyer Buyer { get; }
        [DataMember] public List<Seller> Sellers { get; private set; }
        [DataMember] public List<Seller> Complited;
        [DataMember] private bool _enabled;

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
        }


        private bool AllowBuy(CoursePoint pt)
        {
            if (!Sellers.Any()) return true;
            var lastSeller = Sellers.Last();
            return lastSeller.BuyOrder.Price > pt.Course * (1 + Tracker.Sett.Delta);
        }
        private void DeleteComplitedSellers()
        {
            var deleted = new List<Seller>();
            foreach (Seller seller in Sellers)
            {
                if (seller.SellOrder.IsComplited)
                {
                    Complited.Add(seller);
                    deleted.Add(seller);                    
                }
            }
            foreach (Seller seller in deleted)
                Sellers.Remove(seller);            
        }

        private void TrySell(CoursePoint pt)
        {
            foreach(var seller in Sellers)
                seller.TrySell(pt, Tracker.MulGradient);            
        }
        public void Trade(CoursePoint curCourse)
        {

            DeleteComplitedSellers();
            switch (Tracker.Track(curCourse))
            {
                case EndPoint.None:
                    if (Tracker.Leap.Mode == TrackMode.Neutral) TrySell(curCourse);
                    break;
                case EndPoint.Up:
                    TrySell(curCourse); break;
                case EndPoint.Down:
                    if (AllowBuy(curCourse))
                        Buyer.Buy(curCourse, Tracker); break;
            }
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
