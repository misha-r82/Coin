using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin.History;
using Lib;

namespace Coin
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
        public DatePeriod[] periods;
        public List<Gradient.Grad> grads = new List<Gradient.Grad>();

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

        public MultiPeriodGrad(Market market, DateTime date, int n1, int n2, bool skv = false)
        {
            Sett = new MultiPeriodSettings(new TimeSpan(0,0,5,0,1), 5, 5);
            periods = GetPeriods(date);
            var data = market.GetData(periods[0]).ToArray();
            int first = n1;
            if (n1 == 0)
            {
                grads.Add(new Gradient.Grad(new[] {data.Last()}));
                first = 1;
            }                
            for (int i = first; i < n2 + 1; i++)
            {
                data = market.GetData(periods[i -1]).ToArray();
                if (skv) grads.Add(new  Gradient.GradSkv(data));
                else grads.Add(new Gradient.Grad(data));
                //Debug.WriteLine("[{0}]={1}", i,grads[i]);
            }
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
        public Gradient.Grad GetGrad()
        {
            double positive = grads.Sum(g => g.GPos)/grads.Count;
            double negative = grads.Sum(g => g.GNeg) / grads.Count;
            double neutral = grads.Sum(g => g.G) / grads.Count;
            return new Gradient.Grad(positive, negative, neutral);
        }
        //при усреднении гралинтов за периоды берется модуль
        public Gradient.Grad GetGradAbs()
        {
            double positive = grads.Sum(g => g.GPos) / grads.Count;
            double negative = grads.Sum(g => g.GNeg) / grads.Count;
            double neutral = grads.Sum(g => Math.Abs(g.G)) / grads.Count;
            return new Gradient.Grad(positive, negative, neutral);
        }

    }
}
