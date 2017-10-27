using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Btr.History;
using Lib;

namespace Btr
{
    public struct MultiPeriodSettings
    {
        public MultiPeriodSettings(TimeSpan t0, int periodCount, double periodMult)
        {
            T0 = t0;
            PeriodCount = periodCount;
            PeriodMult = periodMult;
        }

        public TimeSpan T0 { get; set; }
        public int PeriodCount { get; set; }
        public double PeriodMult { get; set; }
    }
    public class MultiPeriodGrad
    {
        public static MultiPeriodSettings Sett;

        public static TimeSpan MaxPeriod
        {
            get
            {
                var t = Sett.T0;
                for (int i = 0; i < Sett.PeriodCount; i++)
                    t = new TimeSpan((long) (t.Ticks * Sett.PeriodMult));
                return t;
            }
        }

        static MultiPeriodGrad()
        {
            Sett = new MultiPeriodSettings(new TimeSpan(0,0,12,0), 1, 1);
        }
        private static DatePeriod[] GetPeriods(DateTime t0)
        {
            DatePeriod[] periods = new DatePeriod[Sett.PeriodCount];
            TimeSpan T = Sett.T0;
            DateTime to = t0;
            for (int i = 0; i < Sett.PeriodCount; i++)
            {
                periods[i] = new DatePeriod(to - T, to);
                T = new TimeSpan((long)(T.Ticks * Sett.PeriodMult));
                to = periods[i].From;
            }
            return periods;
        }
        public static Gradient.Grad GetGradSkv(Market market, DateTime date)
        {
            var periods = GetPeriods(date);
            var grads = new Gradient.Grad[periods.Length +1];
            var data = market.GetData(periods[0]).ToArray();
            grads[0] = new Gradient.Grad(new []{data.Last()} );
            grads[1] = new Gradient.GradSkv(data);
            for (int i = 1; i < grads.Length - 1; i++)
            {
                data = market.GetData(periods[i]).ToArray();
                grads[i+1] = new  Gradient.GradSkv(data);
               
            }
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowMGrad))
            {
                for (int i = 0; i < grads.Length; i++)
                    Debug.Write(string.Format("[{0}]={1}", i, grads[i]));
                Debug.Write("\r\n");
            }
            double positive = grads.Sum(g => g.GPos)/grads.Length;
            double negative = grads.Sum(g => g.GNeg) / grads.Length;
            double neutral = grads.Sum(g => g.G) / grads.Length;
            return new Gradient.Grad(positive, negative, neutral);

        }

    }
}
