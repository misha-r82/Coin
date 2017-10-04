using System;
using System.Diagnostics;
using Lib.Annotations;

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

        public void TrySell(CoursePoint point, Gradient.Grad grad)
        {
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * _sett.Delta;
            if (minDelta < _sett.Delta) minDelta = _sett.Delta;
            /*Debug.WriteLine("bout={0:0.00000} curse={1:0.00000} delta={2:0.00000}",
                BoughtPt.Course, point.Course, point.Course - BoughtPt.Course);*/
            if (point.Course < BoughtPt.Course *(1 + minDelta)) return;
            SellPoint = point;
            Selled = true;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowSell))
            {
                var mrg = point.Course - BoughtPt.Course * (1 + _sett.Delta);
                Debug.WriteLine(string.Format("sell:{0} buy:{1} kd={2:0.000 00} mrg={3:0.000 00} mrg={4:0.000 00}",
                        point, BoughtPt, (point.Course - BoughtPt.Course)/(_sett.Delta * BoughtPt.Course), mrg, mrg/BoughtPt.Course * 100));
            }

            
        }

    }
}