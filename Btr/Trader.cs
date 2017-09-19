using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public CoursePoint BoutPt { get; private set; }

        public List<Seller> Sellers { get; private set; }
        public List<Seller> Complited;

        public void Trade(CoursePoint curCourse)
        {
            var enumerator = Sellers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var seller = enumerator.Current;
                {
                    Complited.Add(seller);
                    Sellers.Remove(seller);
                }
            }


            switch (_tracker.Track(curCourse))
            {
                case EndPoint.Up:
                    foreach(var seller in Sellers)
                        seller.TrySell(_tracker.Leap.UpEnd);
                    break;
                case EndPoint.Down: Buy(_tracker.Leap.DownEnd); break;
                case EndPoint.None: 
                    foreach (var seller in Sellers)
                    {
                        seller.TrySell(curCourse);

                    }
                    break;
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
