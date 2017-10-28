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
            var gNow = MultiPeriodGrad.GetGradSkv(_market, course.Date, 0, 1);
            var gPrew = MultiPeriodGrad.GetGradSkv(_market, course.Date, 1, MultiPeriodGrad.Sett.PeriodCount);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.WriteLine("{0} {1:0.000000} {2} {3}", 
                    course, gNow, gPrew, Leap.Mode);
            //Debug.WriteLine("g={0} m={1} {2}", g, MulGradient, Leap.Mode);
            //Debug.WriteLine("g+={0} g-={1} {2}", g.GNeg/MulGradient.GNeg, g.GPos/MulGradient.GPos, Leap.Mode);
            if (Math.Abs((gNow.G - _lastGrad)/ (gNow.G + _lastGrad)) < _sett.GGap)
                return EndPoint.None;
            _lastGrad = gPrew.G;
            if (gNow.G > gPrew.GPos)
                return Leap.SetUp(course);
            if (gNow.G < gPrew.GNeg)
                return Leap.SetDown(course);
            return Leap.SetNeutral(course);
        }
    }
}
