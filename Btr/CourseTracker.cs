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
        public TrackMode Mode { get; set; }
        public CourseTracker(Market market, BaseSettings sett)
        {
            _market = market;
            Mode = TrackMode.Neutral;
        }
        public enum TrackMode { Neutral, Down, Up}

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
            var period = new DatePeriod(tStart - T, tStart);
            double g = GetGradient(period);
            if (Math.Abs(g) < _sett.Delta)
                Mode = TrackMode.Neutral;
            else Mode = g > 0 ? TrackMode.Up : TrackMode.Down;
        }
    }
}
