using System;
using System.Linq;
using System.Resources;

namespace Coin
{
    public enum TrackMode { Neutral, Down, Up, Error }
    public enum EndPoint { None, DownEnd, UpEnd}
    public class LeapInfo
    {
        public CoursePoint UpBegin { get; private set; }
        public CoursePoint UpEnd { get; private set; }
        public CoursePoint DownBegin { get; private set; }
        public CoursePoint DownEnd { get; private set; }

        public TrackMode Mode { get; private set; }

        public CoursePoint LastPt
        {
            get
            {
                var points = new  []{ UpBegin, UpEnd, DownBegin, DownEnd };
                var maxTime = points.Max(pt => pt.Date);
                return points.First(pt => pt.Date == maxTime);
            }
        }

        public LeapInfo()
        {
            Mode = TrackMode.Error;
        }
        public EndPoint SetUp(CoursePoint course, double delta)
        {
            switch (Mode)
            {
                case TrackMode.Neutral:
                    Mode = TrackMode.Up;
                    UpBegin = course;
                    return EndPoint.None;
                case TrackMode.Down:
                    Mode = TrackMode.Up;
                    if (Math.Abs(LastPt.Course - course.Course) < delta) return EndPoint.None;
                    DownEnd = course;
                    return EndPoint.DownEnd;
                default: return EndPoint.None;
            }
        }

        public EndPoint SetDown(CoursePoint course, double delta)
        {
            switch (Mode)
            {
                case TrackMode.Neutral:
                    Mode = TrackMode.Down;
                    DownBegin = course;
                    return EndPoint.None;
                case TrackMode.Up:
                    Mode = TrackMode.Down;
                    if (Math.Abs(LastPt.Course - course.Course) < delta) return EndPoint.None;
                    UpEnd = course;
                    return EndPoint.UpEnd;
                    default: return EndPoint.None;
            }
        }

        public EndPoint SetNeutral(CoursePoint course, double delta)
        {
            if (Math.Abs(LastPt.Course - course.Course) < delta) return EndPoint.None;
            Mode = TrackMode.Neutral;
            if (Mode == TrackMode.Error) // первый запуск
            {
                if (Mode == TrackMode.Down)
                {
                    DownEnd = course;
                    return EndPoint.None;
                }
                else
                {
                    UpEnd = course;
                    return EndPoint.None;
                }
            }
            // основной режим

            if (course.Course - LastPt.Course < 0)
            {
                DownEnd = course;
                return EndPoint.DownEnd;
            } 
            UpBegin = course;
            return EndPoint.UpEnd;
        }
    }
}