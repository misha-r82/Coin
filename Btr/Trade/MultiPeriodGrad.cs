﻿using System;
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
        public static Gradient.Grad GetGradSkv(Market market, DateTime date, int n1, int n2)
        {
            var periods = GetPeriods(date);
            var grads = new Gradient.Grad[n2-n1+1];
            var data = market.GetData(periods[0]).ToArray();
            int first = n1;
            if (n1 == 0)
            {
                grads[0] = new Gradient.Grad(new[] {data.Last()});
                first = 1;
            }                
            for (int i = first; i < grads.Length; i++)
            {
                data = market.GetData(periods[i]).ToArray();
                grads[i] = new  Gradient.GradSkv(data);
                //Debug.WriteLine("[{0}]={1}", i,grads[i]);
            }
            double positive = grads.Sum(g => g.GPos)/grads.Length;
            double negative = grads.Sum(g => g.GNeg) / grads.Length;
            double neutral = grads.Sum(g => g.G) / grads.Length;
            return new Gradient.Grad(positive, negative, neutral);

        }

    }
}
