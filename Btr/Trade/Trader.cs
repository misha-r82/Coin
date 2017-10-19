using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Btr
{
    public enum TradeMode { Wait, Buy, Sell }
    public class Treader
    {
        CourseTracker _tracker;
        public CoursePoint BuyPoint { get; private set; }
        public Treader(CourseTracker tracker)
        {
            _tracker = tracker;
            Complited = new List<Seller>();
            Sellers = new List<Seller>();
        }

        public List<Seller> Sellers { get; private set; }
        public List<Seller> Complited;
        public double Balance { get; set; }
        public double PartsInvest { get; set; }
        public bool AllowBuy(CoursePoint pt)
        {
            if (!Sellers.Any()) return true;
            var lastSeller = Sellers.Last();
            return lastSeller.BoughtPt.Course > pt.Course * (1 + _tracker.Sett.Delta);
        }
        private void DeleteComplitedSellers()
        {
            var deleted = new List<Seller>();
            foreach (Seller seller in Sellers)
            {
                if (seller.Selled)
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
                seller.TrySell(pt, _tracker.MulGradient);            
        }
        public void Trade(CoursePoint curCourse)
        {

            DeleteComplitedSellers();

            switch (_tracker.Track(curCourse))
            {
                case EndPoint.None:
                    if (_tracker.Leap.Mode == TrackMode.Neutral) TrySell(curCourse);
                    break;
                case EndPoint.Up:
                    TrySell(curCourse); break;
                case EndPoint.Down:
                    if (AllowBuy(curCourse))
                        Buy(curCourse, _tracker.MulGradient, _tracker.Leap); break;
            }
        }

        private void Buy(CoursePoint buyPoint, Gradient.Grad grad, LeapInfo leap)
        {
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * _tracker.Sett.Delta;

            if (minDelta < _tracker.Sett.Delta) minDelta = _tracker.Sett.Delta;
            if (buyPoint.Course > leap.DownBegin.Course * (1 - minDelta)) return;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowBuy))
                Debug.WriteLine(string.Format("Buy={0} {1} g+/g-={2:0.00000}", buyPoint, _tracker.Leap.Mode, grad.GPos / grad.GNeg));
            Sellers.Add(new Seller(_tracker.Market, buyPoint, _tracker.Sett));
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
