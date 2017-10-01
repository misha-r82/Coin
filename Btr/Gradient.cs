using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using Btr.History;
using Lib;

namespace Btr
{
    public class Gradient
    {
        public struct Grad
        {
            public Grad(double gpos, double gneg, double g)
            {
                GPos = gpos;
                GNeg = gneg;
                G = g;
            }
            public double GPos { get; }
            public double GNeg { get; }
            public double G { get ;  }
            public override string ToString()
            {
                return string.Format("g:{0:#.000000} g+:{1:#.000000} g-:{2:#.000000}", G, GPos, GNeg);
            }
        }

        public class DeltasSkv
        {
            public double GPos { get; }
            public double GNeg { get; }
            public double G { get; }
            public DeltasSkv(PlnCouse.CouseItem[] data)
            {
                var deltaArr = new DeltaArr(data);
                GPos = Math.Sqrt(deltaArr.positive.Sum(d => d * d) / deltaArr.positive.Length);
                GPos = Math.Sqrt(deltaArr.negative.Sum(d => d * d) / deltaArr.negative.Length);
                G = Math.Sqrt(deltaArr.all.Sum(d => d * d) / deltaArr.all.Length);
            }
        }

        public class Deltas
        {
            public double GPos { get; }
            public double GNeg { get; }
            public double G { get; }
            public Deltas(PlnCouse.CouseItem[] data)
            {
                var deltaArr = new DeltaArr(data);
                GPos = deltaArr.positive.Length ==0 ? 0 : deltaArr.positive.Sum() / deltaArr.positive.Length;
                GNeg = deltaArr.negative.Length == 0 ? 0 : deltaArr.negative.Sum() / deltaArr.negative.Length;
                G = GPos + GNeg;
            }
        }
        public static Grad GetGradient(PlnCouse.CouseItem[] data, DatePeriod period, TimeSpan tBase)
        {
            int count = data.Length;
            if (count == 0) return new Grad(0,0,0);
            double kT = period.Dlit.TotalMilliseconds / tBase.TotalMilliseconds;
            if (count == 1) return data[0].delta < 0 ? 
                new Grad(0, data[0].delta/kT, data[0].delta / kT) : 
                new Grad(data[0].delta/kT, 0, data[0].delta / kT);
            var deltas = new Deltas(data);
            return new Grad(deltas.GPos/kT, deltas.GNeg/kT, deltas.G/kT);
        }


        public static double WndGrad(PlnCouse.CouseItem[] data, DatePeriod period, TimeSpan tBase, double wSlope = 0.6)
        {
            int count = data.Length;
            if (count == 0) return double.NaN;
            if (count == 1) return data[0].delta;
            double g = 0;
            double lastNotNull = 0;
            double w = 1 - wSlope;
            double dw = 1 - wSlope / count;
            for (int i = 0; i < count - 1; i++)
            {
                if (data[i].course != 0) lastNotNull = data[i].course;
                if (lastNotNull > 0 && data[i + 1].course > 0)
                    g += w * (data[i + 1].course - data[i].course);
                w += dw;
            }
            double kT = period.Dlit.TotalMilliseconds / tBase.TotalMilliseconds;
            return g / kT / (1 - 0.5 * wSlope);
        }

        public class DeltaArr
     {
         public double[] negative;
         public double[]  positive;
         public double[] all;
         public DeltaArr(PlnCouse.CouseItem[] data)
         {
             int count = data.Length;
             var pos = new List<double>();
             var neg = new List<double>();
             var all = new List<double>();
             double lastNotNull = 0;
             for (int i = 0; i < count - 1; i++)
             {
                 if (data[i].course != 0) lastNotNull = data[i].course;
                 if (lastNotNull > 0 && data[i + 1].course > 0)
                 {
                     double notNull = data[i].course > 0 ? data[i].course : lastNotNull;
                     var g = data[i + 1].course - notNull;
                     if (g < 0) neg.Add(g);
                     else pos.Add(g);
                     all.Add(g);
                 }
             }
             negative = neg.ToArray();
             positive = pos.ToArray();
             this.all = all.ToArray();
         }
    }
        
}
}

