using Coin.History;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Btr.Log;
using Btr.Trade;
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

        [DataMember] public LeapInfo Leap { get; private set; }
        public Gradient.Grad G1 { get; private set; }
        public TrackSettings Sett
        {
            get { return _sett; }
            set { _sett = value; }
        }
        public Market Market
        {
            get { return _market; }
            set => _market = value;
        }

        public EndPoint Track(CoursePoint course)
        {
            if (_prewPt.Date == new DateTime()) _prewPt = course;
            if (course.Date == new DateTime(2018, 04, 13, 07, 50, 0))
            {}           
            if (course.Course == 0) return EndPoint.None;
            //var gTest = MultiPeriodGrad.GetGrad(_market, course.Date, 0, MultiPeriodGrad.Sett.PeriodCount);
            Gradient.Grad G0 = new MultiPeriodGrad(_market, course.Date, 1, 2).GetGrad();
            var G1 = new MultiPeriodGrad(_market, course.Date, 3, MultiPeriodGrad.Sett.PeriodCount).GetGradAbs();
            var prewCopy = _prewPt;
            _prewPt = course;
            double Gbase = G1.G;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowCourse))
                Log.CreateLog("Track", string.Format("{0} {1} {2} ", course, G0.G, Gbase));
            double smartK = G0.G > 0 ? Math.Abs(G1.GPos / G1.GNeg) : Math.Abs(G1.GNeg / G1.GPos);
            smartK = 1;
            double delta = smartK * _sett.Delta * course.Course;
            if (Math.Abs(G0.G) < _sett.Delta * smartK * course.Course / 10) return Leap.SetNeutral(course, delta);
            if (Math.Abs(G0.G / Gbase) < _sett.KGrad * smartK) return Leap.SetNeutral(course, delta);
            //if (Math.Abs(G0.G / G2.G) < _sett.KGrad * 2) return Leap.SetNeutral(course);
            if (G0.G > 0/* && (course.Course - Leap.LastPt.Course > _sett.Delta * smartK || Leap.LastPt.Date > new DateTime(0))*/) return Leap.SetUp(prewCopy, delta);
            if (G0.G < 0/* && (course.Course - Leap.LastPt.Course > _sett.Delta * smartK || Leap.LastPt.Date > new DateTime(0))*/) return Leap.SetDown(prewCopy, delta);
            return Leap.SetNeutral(course, delta);
        }
    }
}
