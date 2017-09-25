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

        public EndPoint Track(CoursePoint course)
        {
            if (course.Date == new DateTime(2017, 09, 04, 06, 20, 0))
            {
                
            }
            var T = _sett.T0;
            if (course.Course == 0) return EndPoint.None;
            var period = new DatePeriod(course.Date - T, course.Date);
            var period1 = new DatePeriod(period.From - T1, period.From);
            var data = _market.GetData(period).ToArray();
            var data1 = _market.GetData(period1).ToArray();
            double g = Gradient.WndGrad(data, period, T);
            var g1 = Gradient.GetGradient(data1, period1, T1);
            if (g == double.NaN) return EndPoint.None;

            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("{0} {1:#.000000} {2:#.000000}", 
                    course.Date,  g, Leap.Mode);
            if (Math.Abs(g - _lastGrad) < delta * _sett.GGap)
                return EndPoint.None;
            _lastGrad = g;
            double positiveDelta = delta + 0.6 * g1;
            double negativeDelta = delta - 0.6 * g1;
            if (g > positiveDelta)
                return Leap.SetUp(course);
            if (g < negativeDelta)
                return Leap.SetDown(course);
            return Leap.SetNeutral(course);
        }
    }
}
