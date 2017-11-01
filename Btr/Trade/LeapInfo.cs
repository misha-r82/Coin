using System;
using System.Resources;

namespace Btr
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

        public LeapInfo()
        {
            Mode = TrackMode.Error;
        }
        public EndPoint SetUp(CoursePoint course)
        {
            if (Mode == TrackMode.Up) return EndPoint.None;
            UpBegin = course;
            var prevMode = Mode;
            Mode = TrackMode.Up;
            if (prevMode == TrackMode.Down)
            {
                DownEnd = course;
                return EndPoint.DownEnd;
            }
            return EndPoint.None;
        }

        public EndPoint SetDown(CoursePoint course)
        {
            if (Mode == TrackMode.Down) return EndPoint.None;
            DownBegin = course;
            var prevMode = Mode;
            Mode = TrackMode.Down;
            if (prevMode == TrackMode.Up)
            {
                UpEnd = course;
                return EndPoint.UpEnd;                
            }
            return EndPoint.None;
        }

        public EndPoint SetNeutral(CoursePoint course)
        {
            var prevMode = Mode;
            Mode = TrackMode.Neutral;
            switch (prevMode)
            {
                    case TrackMode.Up: UpEnd = course;
                       return EndPoint.UpEnd;
                    case TrackMode.Down:
                        DownEnd = course;
                        return EndPoint.DownEnd;
            }
            return EndPoint.None;
        }
    }
}