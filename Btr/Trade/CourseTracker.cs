using Btr.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Test.Annotations;

namespace Btr
{
    [DataContract]
    public class CourseTracker
    {
        [DataMember] private Market _market;
        [DataMember] private TrackSettings _sett;
        private double _lastGrad;
        public CourseTracker(Market market, TrackSettings sett)
        {
            _market = market;
            _sett = sett;
            _lastGrad = 0;
            Leap = new LeapInfo();
        }

        public LeapInfo Leap { get; }
        public Gradient.Grad MulGradient { get; private set; }
        public TrackSettings Sett
        {
            get { return _sett; }
        }

        public Market Market
        {
            get { return _market; }
        }

        public EndPoint Track(CoursePoint course)
        {
            if (course.Date > new DateTime(2017, 09, 04, 21, 40, 0))
            {
                
            }           
            if (course.Course == 0) return EndPoint.None;
            var period = new DatePeriod(course.Date - MultiPeriodGrad.Sett.T0, course.Date);
            var g = new Gradient.Grad(_market.GetData(period).ToArray());
            MulGradient = MultiPeriodGrad.GetGradSkv(_market, course.Date);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("{0} {1:0.000000} {2} {3}", 
                    course, g, MulGradient, Leap.Mode);
            //Debug.WriteLine("g={0} m={1} {2}", g, MulGradient, Leap.Mode);
            //Debug.WriteLine("g+={0} g-={1} {2}", g.GNeg/MulGradient.GNeg, g.GPos/MulGradient.GPos, Leap.Mode);
            if (Math.Abs((g.G - _lastGrad)/_lastGrad) < _sett.GGap)
                return EndPoint.None;
            _lastGrad = MulGradient.G;
            if (g.G > MulGradient.GPos)
                return Leap.SetUp(course);
            if (g.G < MulGradient.GNeg)
                return Leap.SetDown(course);
            return Leap.SetNeutral(course);
        }
    }
}
