using System;
using System.Collections.Generic;
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
                case EndPoint.Up:
                    TrySell(curCourse); break;
                case EndPoint.Down:
                    Buy(_tracker.Leap.DownEnd); break;
                case EndPoint.None: 
                    TrySell(curCourse); break;
            }
        }

        private void Buy(CoursePoint buyPoint)
        {
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

    }
    
    public class Seller
    {
        public Market Market { get; }
        public CoursePoint BoughtPt { get; }
        public CoursePoint SellPoint { get; private set; }
        public bool Selled { get; private set; }
        private BaseSettings _sett;
        
        public Seller(Market market, CoursePoint boughtPt, BaseSettings sett)
        {
            Market = market;
            BoughtPt = boughtPt;
            _sett = sett;            
        }

        public void TrySell(CoursePoint point)
        {
            if (point.Course > BoughtPt.Course + _sett.Delta)
            {
                SellPoint = point;
                Selled = true;
            }
        }

    }
}
