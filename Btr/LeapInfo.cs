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
        public EndPoint SetUp(CoursePoint course)
        {
            if (Mode == TrackMode.Up) return EndPoint.None;
            UpBegin = course;
            EndPoint result = EndPoint.None;
            if (Mode == TrackMode.Down)
            {
                DownEnd = course;
                result = EndPoint.Down;
            }
            Mode = TrackMode.Up;
            return result;
        }

        public EndPoint SetDown(CoursePoint course)
        {
            if (Mode == TrackMode.Down) return EndPoint.None;
            DownBegin = course;
            EndPoint result = EndPoint.None;
            if (Mode == TrackMode.Up)
            {
                UpEnd = course;
                result = EndPoint.Up;                
            }
            Mode = TrackMode.Down;
            return result;
        }

        public EndPoint SetNeutral(CoursePoint course)
        {
            switch (Mode)
            {
                    case TrackMode.Up: UpEnd = course;
                       return EndPoint.Up;
                    case TrackMode.Down: DownEnd = course;
                        return EndPoint.Down;
            }
            return EndPoint.None;
        }
    }
}