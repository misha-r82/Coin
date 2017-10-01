using System;
using System.Collections.Generic;
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

        static MultiPeriodGrad()
        {
            Sett = new MultiPeriodSettings(new TimeSpan(0,0,12,0), 5, 5);
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
            var grads = new Gradient.Grad[periods.Length];
            for (int i = 0; i < grads.Length; i++)
            {
                var data = market.GetData(periods[i]).ToArray();
                grads[i] = Gradient.GetGradient(data, periods[i], Sett.T0);
            }
            double positive = grads.Sum(g => g.GPos)/grads.Length;
            double negative = grads.Sum(g => g.GNeg) / grads.Length;
            double neutral = grads.Sum(g => g.G) / grads.Length;
            return new Gradient.Grad(positive, negative, neutral);

        }

    }
}
