using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using Btr.PrivApi;

namespace Btr
{
    public enum TradeMode { Wait, Buy, Sell }
    [DataContract]
    public class Treader
    {

        private CourseTracker _tracker;
        private ApiParser _apiParser { get; }
        [DataMember]public Buyer Buyer { get; }
        [DataMember] public List<Seller> Sellers { get; private set; }
        [DataMember] public List<Seller> Complited;
        public Treader(CourseTracker tracker, ApiParser apiParser)
        {
            _tracker = tracker;
            _apiParser = apiParser;
            Buyer = new Buyer(apiParser);
            Complited = new List<Seller>();
            Sellers = new List<Seller>();
        }


        private bool AllowBuy(CoursePoint pt)
        {
            if (!Sellers.Any()) return true;
            var lastSeller = Sellers.Last();
            return lastSeller.BuyOrder.Price > pt.Course * (1 + _tracker.Sett.Delta);
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
                        Buyer.Buy(curCourse, _tracker); break;
            }
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
