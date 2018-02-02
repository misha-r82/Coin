using Coin.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Test.Annotations;

namespace Coin
{
    [DataContract]
    public class CourseTracker
    {
        [DataMember] private Market _market;
        [DataMember] private TrackSettings _sett;
        [DataMember] private CoursePoint _prewPt;
        public CourseTracker(Market market, TrackSettings sett)
        {
            _market = market;
            _sett = sett;
            Leap = new LeapInfo();
        }

        public LeapInfo Leap { get; }
        public Gradient.Grad GPrew { get; private set; }
        public TrackSettings Sett { get { return _sett; } }
        public Market Market { get { return _market; } }

        public EndPoint Track(CoursePoint course)
        {
            if (_prewPt.Date == new DateTime()) _prewPt = course;
            if (course.Date == new DateTime(2017, 10, 24, 0, 15, 0))
            {}           
            if (course.Course == 0) return EndPoint.None;
            //var gTest = MultiPeriodGrad.GetGradSkv(_market, course.Date, 0, MultiPeriodGrad.Sett.PeriodCount);
            var gNow = MultiPeriodGrad.GetGradSkv(_market, course.Date, 1, 1);
            GPrew = MultiPeriodGrad.GetGradSkv(_market, course.Date, 2, MultiPeriodGrad.Sett.PeriodCount);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Debug.Write(string.Format("{0} {1} {2} ", course, gNow, GPrew));
            var prewCopy = _prewPt;
            _prewPt = course;
            if (gNow.G > GPrew.GPos * _sett.GGap)
                return Leap.SetUp(prewCopy);
            if (gNow.G < GPrew.GNeg * _sett.GGap)
                return Leap.SetDown(prewCopy);
            return Leap.SetNeutral(course);
        }
    }
}
