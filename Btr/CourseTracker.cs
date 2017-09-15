using Btr.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{

    class CourseTracker
    {
        private Market _market;
        private BaseSettings _sett;
        public CourseTracker(Market market, BaseSettings sett)
        {
            _market = market;
            Leap = new LeapInfo();
        }

        public LeapInfo Leap { get; }

        public PlnCouse.CouseItem[] GetData(DatePeriod period)
        {
            return _market.CourseData.Where(x => period.IsConteins(x.date)).ToArray();
        }
        public double GetGradient(DatePeriod period)
        {
            var data = GetData(period);
            int count = data.Length;
            double g = 0;
            double sred = data[0].course / count;
            for (int i = 0; i < count - 1; i++)
            {
                g += data[i + 1].course - data[i].course;
                sred += data[i + 1].course / count;
            }

            double kT = period.Dlit.TotalMilliseconds / _sett.T.TotalMilliseconds;
            return g / kT;
        }
        public void Track(DateTime tStart)
        {
            var T = _sett.T;
            double course = _market.Getourse(tStart);
            var period = new DatePeriod(tStart - T, tStart);
            double g = GetGradient(period);
            if (Math.Abs(g) < _sett.Delta)
                Leap.SetNeutral(tStart, course);
            else
                if (g > 0) Leap.SetUp(tStart, course);
                else Leap.SetDown(tStart, course);
        }
    }
}
