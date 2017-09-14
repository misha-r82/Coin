using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace Btr
{
    public class Leap
    {
        public Leap(double g1, double g2, DatePeriod p1, DatePeriod p2)
        {
            Period1 = p1;
            Period2 = p2;
            G1 = g1;
            G2 = g2;
        }
        public DatePeriod Period1 { get; }
        public DatePeriod Period2 { get; }
        public double G1 { get; }
        public double G2 { get; }
    }
}
