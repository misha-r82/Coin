using System;

namespace Btr
{
    public enum TrackMode { Neutral, Down, Up, Error }
    public class LeapInfo
    {

        public double UpBegin { get; private set; }
        public double UpEnd { get; private set; }
        public double DownBegin { get; private set; }
        public double DownEnd { get; private set; }
        public DateTime UpBeginTime { get; private set; }
        public DateTime UpEndTime { get; private set; }
        public DateTime DownBeginTime { get; private set; }
        public DateTime DownEndTime { get; private set; }

        public TrackMode Mode { get; private set; }

        public LeapInfo()
        {
            Mode = TrackMode.Error;
        }
        public void SetUp(DateTime time, double course)
        {
            if (Mode == TrackMode.Up) return;
            UpBegin = course;
            UpBeginTime = time;
            if (Mode == TrackMode.Down)
            {
                DownEnd = course;
                DownEndTime = time;
            }
            Mode = TrackMode.Up;
        }

        public void SetDown(DateTime time, double course)
        {
            if (Mode == TrackMode.Down) return;
            DownBegin = course;
            DownBeginTime = time;
            if (Mode == TrackMode.Up)
            {
                UpEnd = course;
                UpEndTime = time;                
            }
            Mode = TrackMode.Down;
        }

        public void SetNeutral(DateTime time, double course)
        {
            switch (Mode)
            {
                    case TrackMode.Neutral: return;
                    case TrackMode.Up: UpEnd = course;
                       UpEndTime = time; break;
                    case TrackMode.Down: DownEnd = course;
                        DownEndTime = time; break;
            }
        }
    }
}