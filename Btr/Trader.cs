using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public enum TradeMode { Wait, Buy, Sell }
    class Treader
    {
        CourseTracker _tracker;
        public CoursePoint BuyPoint { get; private set; }
        public Treader(CourseTracker tracker)
        {
            _tracker = tracker;
            Complited = new List<Seller>();
        }
        public CoursePoint BoutPt { get; private set; }

        public Seller Seller { get; private set; }
        public List<Seller> Complited;

        public void Trade(DateTime start)
        {
            if (Seller.Selled)
            {
                Complited.Add(Seller);
                Seller = null;
            }


            switch (_tracker.Track(start))
            {
                case EndPoint.Up: Seller.TrySell(_tracker.Leap.UpEnd); break;
                case EndPoint.Down: Buy(_tracker.Leap.DownEnd); break;
                case EndPoint.None: if (Seller != null) Seller.TrySell();
            }
        }

        private void Buy(CoursePoint buyPoint)
        {
            BoutPt = buyPoint;
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
