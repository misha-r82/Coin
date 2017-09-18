using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Btr.History;
using Lib;

namespace Btr
{
    class LeapFounder
    {
       /* public LeapFounder(BaseSettings sett)
        {
            BaseSett = sett;
        }
        public Market Market { get; set; }
        public TimeSpan MaxPeriod { get; set; }
        public BaseSettings BaseSett {get;}

        public PlnCouse.CouseItem[] GetData(DatePeriod period)
        {
            return Market.CourseData.Where(x => period.IsConteins(x.date)).ToArray();
        }
        public double GetGradient(DatePeriod period)
        {
            var data = GetData(period);
            int count = data.Length;
            double g = 0;
            double sred = data[0].course / count;
            for (int i = 0; i < count - 1; i++)
            {
                g += data[i + 1].course - data[i].course;
                sred += data[i + 1].course / count;
            }
                
            double kT = period.Dlit.TotalMilliseconds / BaseSett.T.TotalMilliseconds;
            return g / kT;
        }*/
        /*public double SumDelta(DatePeriod period)
        {
            var data = GetData(period);
            double s = 0;
            for (int i = 0; i < data.Length - 1; i++)
                s += data[i + 1].course - data[i].course;
            return s;
        }*/
        /*public Leap FindLeap(DateTime start)
        {
            var T = BaseSett.T;
            var period = new DatePeriod(start - T, start);
            double s = SumDelta(period);
                double g2 = GetGradient(period2);
                if (g1 > GRatio * g2) return new Leap(g1, g2, period1, period2);
                if (g1 < g2/GRatio) return new Leap(g1, g2, period1, period2);
                var allTime = period1.Dlit + period2.Dlit;
                if (allTime > MaxPeriod) return null;
            } while (true);

        }*/
    }
}
