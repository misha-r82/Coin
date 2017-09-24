using Btr.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{

    public class CourseTracker
    {
        private Market _market;
        private BaseSettings _sett;
        private double _lastGrad;
        public CourseTracker(Market market, BaseSettings sett)
        {
            _market = market;
            _sett = sett;
            _lastGrad = 0;
            Leap = new LeapInfo();
        }

        public LeapInfo Leap { get; }

        public BaseSettings Sett
        {
            get { return _sett; }
        }

        public Market Market
        {
            get { return _market; }
        }


        public double GetGradient(DatePeriod period)
        {
            var data = _market.GetData(period).ToArray();
            int count = data.Length;
            if (count == 0) return double.NaN;
            double g = 0;
            int nullCount = data.Count(d => d.course == 0);
            int countEff = count - nullCount;
            double sred = data[0].course / countEff;
            double lastNotNull = 0;
            for (int i = 0; i < count - 1; i++)
            {
                if (data[i].course != 0) lastNotNull = data[i].course;
                if (lastNotNull > 0 && data[i+1].course > 0)
                    g += data[i + 1].course - data[i].course;
                sred += data[i + 1].course / countEff;
            }

            double kT = period.Dlit.TotalMilliseconds / _sett.Tbase.TotalMilliseconds;
            return g / kT;
        }
        public double WndGrad(DatePeriod period)
        {
            double wSlope = 0.6;
            var data = _market.GetData(period).ToArray();
            int count = data.Length;
            if (count == 0) return double.NaN;
            double g = 0;
            double lastNotNull = 0;
            double w = 1 - wSlope;
            double dw = wSlope / count;
            for (int i = 0; i < count - 1; i++)
            {
                if (data[i].course != 0) lastNotNull = data[i].course;
                if (lastNotNull > 0 && data[i + 1].course > 0)
                    g += w * (data[i + 1].course - data[i].course);
                w += dw;
            }

            double kT = period.Dlit.TotalMilliseconds / _sett.Tbase.TotalMilliseconds;
            return g / kT / (1 - 0.5 * wSlope);
        }
        public EndPoint Track(CoursePoint course)
        {
            if (course.Date == new DateTime(2017, 09, 04, 06, 20, 0))
            {
                
            }
            var T = _sett.T0;
            if (course.Course == 0) return EndPoint.None;
            var period = new DatePeriod(course.Date - T, course.Date);
            double g = WndGrad(period);
            if (g == double.NaN) return EndPoint.None;
            double delta = _sett.Delta / 2;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("{0} {1:#.000000} {2:#.000000}", 
                    course.Date,  g, Leap.Mode);
            if (Math.Abs(g - _lastGrad) < delta * _sett.GGap)
                return EndPoint.None;
            _lastGrad = g;
            if (Math.Abs(g) < delta)
                return Leap.SetNeutral(course);
            if (g > 0)
                return Leap.SetUp(course);
            return Leap.SetDown(course);
        }
    }
}
