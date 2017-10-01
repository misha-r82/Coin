using Btr.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Annotations;

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
            if (course.Date > new DateTime(2017, 09, 04, 18, 00, 0))
            {
                
            }           
            if (course.Course == 0) return EndPoint.None;
            var period = new DatePeriod(course.Date - MultiPeriodGrad.Sett.T0, course.Date);
            var g = Gradient.GetGradient(_market.GetData(period).ToArray(), period, MultiPeriodGrad.Sett.T0);
            var multiGrad = MultiPeriodGrad.GetGradSkv(_market, course.Date);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("{0} {1:#.000000} {2} {3}", 
                    course, g,  multiGrad, Leap.Mode);
            if (Math.Abs((multiGrad.G - _lastGrad)/ (multiGrad.G + _lastGrad)) < _sett.GGap)
                return EndPoint.None;
            _lastGrad = multiGrad.G;
            if (g.G > multiGrad.GPos)
                return Leap.SetUp(course);
            if (g.G < multiGrad.GNeg)
                return Leap.SetDown(course);
            return Leap.SetNeutral(course);
        }
    }
}
