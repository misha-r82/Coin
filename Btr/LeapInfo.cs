using System;
using System.Resources;

namespace Btr
{
    public enum TrackMode { Neutral, Down, Up, Error }
    public enum EndPoint { None, Down, Up}
    public class LeapInfo
    {
        public CoursePoint UpBegin { get; private set; }
        public CoursePoint UpEnd { get; private set; }
        public CoursePoint DownBegin { get; private set; }
        public CoursePoint DownEnd { get; private set; }

        public TrackMode Mode { get; private set; }

        public LeapInfo()
        {
            Mode = TrackMode.Error;
        }
        public EndPoint SetUp(DateTime time, double course)
        {
            if (Mode == TrackMode.Up) return EndPoint.None;
            UpBegin = new CoursePoint(course, time);
            EndPoint result = EndPoint.None;
            if (Mode == TrackMode.Down)
            {
                DownEnd = new CoursePoint(course, time);
                result = EndPoint.Down;
            }
            Mode = TrackMode.Up;
            return result;
        }

        public EndPoint SetDown(DateTime date, double course)
        {
            if (Mode == TrackMode.Down) return EndPoint.None;
            DownBegin = new CoursePoint(course, date);
            EndPoint result = EndPoint.None;
            if (Mode == TrackMode.Up)
            {
                UpEnd = new CoursePoint(course, date);
                result = EndPoint.Up;                
            }
            Mode = TrackMode.Down;
            return result;
        }

        public EndPoint SetNeutral(DateTime date, double course)
        {
            switch (Mode)
            {
                    case TrackMode.Up: UpEnd = new CoursePoint(course, date);
                       return EndPoint.Up;
                    case TrackMode.Down: DownEnd = new CoursePoint(course, date);
                        return EndPoint.Down;
            }
            return EndPoint.None;
        }
    }
}