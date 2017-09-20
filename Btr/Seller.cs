using System.Diagnostics;

namespace Btr
{
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
                //Debug.WriteLine("Sell=" + point.Course);
            }
        }

    }
}