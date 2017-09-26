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
            if (point.Course > BoughtPt.Course * (1 + _sett.Delta))
            {
                SellPoint = point;
                Selled = true;

                if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowSell))
                {
                    var mrg = point.Course - BoughtPt.Course * (1 + _sett.Delta);
                    Debug.WriteLine(string.Format("sell:{0} buy:{1} kd={2:#.000000} mrg={3:#.000000} mrg={4:#.000000}",
                         point, BoughtPt, (point.Course - BoughtPt.Course)/(_sett.Delta * BoughtPt.Course), mrg, mrg/BoughtPt.Course * 100));
                }

            }
        }

    }
}