using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    class Treader
    {
        CourseTracker _tracker;
        public Treader(CourseTracker tracker)
        {
            _tracker = tracker;
        }
        public double BoutCourse { get; private set; }
        public TradeMode Mode { get; private set; }
        public enum TradeMode { Wait, Buy, Sell }

        public void Trade(DateTime start)
        {
            var prewMode = _tracker.Mode;
            _tracker.Track(start);
            switch (Mode)
            {
                case TradeMode.Wait:
                    if (_tracker.Mode == CourseTracker.TrackMode.Down)
                        Mode = TradeMode.Buy;
                        else Mode = TradeMode.Wait;
                    break;
                case TradeMode.Buy: 
                    
            }
        }
    }
}
