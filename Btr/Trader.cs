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
                seller.TrySell(pt);            
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
                        Buy(curCourse); break;
            }
        }

        private void Buy(CoursePoint buyPoint)
        {

            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowBuy))
                Debug.WriteLine(string.Format("Buy={0} {1}", buyPoint, _tracker.Leap.Mode));
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
            return string.Format("{0:dd.MM HH:mm}|{1:#.000000} ", Date, Course);
        }
    }
}
