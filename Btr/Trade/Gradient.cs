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
        public class Grad
        {
            public double GPos { get; protected set; }
            public double GNeg { get; protected set; }
            public double G { get ; protected set; }
            protected Grad() { }
            public Grad(double gpos, double gneg, double g)
            {
                GPos = gpos;
                GNeg = gneg;
                G = g;
            }
            public Grad(CourseItem[] data)
            {
                if (SmallDataConstructor(data)) return;
                var deltaArr = new DeltaArr(data);
                GPos = deltaArr.positive.Length == 0 ? 0 : deltaArr.positive.Sum() / deltaArr.positive.Length;
                GNeg = deltaArr.negative.Length == 0 ? 0 : deltaArr.negative.Sum() / deltaArr.negative.Length;
                G = GPos + GNeg;
            }

            protected bool SmallDataConstructor(CourseItem[] data)
            {
                int count = data.Length;
                if (count == 0)
                {
                    G = GNeg = GPos = 0;
                    return true;
                }
                if (count == 1)
                {
                    if (data[0].delta < 0)
                        GNeg = G = data[0].delta;
                    else
                        GPos = G = data[0].delta;
                    return true;
                }
                return false;
            }

            public override string ToString()
            {
                return string.Format("g:{0:0.00000} g+:{1:0.00000} g-:{2:0.00000}", G, GPos, GNeg);
            }
        }

        public class GradSkv : Grad
        {
            public GradSkv(CourseItem[] data)
            {
                if (SmallDataConstructor(data)) return;
                var deltaArr = new DeltaArr(data);
                GPos = deltaArr.positive.Length == 0 ? 0 : 
                    Math.Sqrt(deltaArr.positive.Sum(d => d * d / deltaArr.all.Length));
                GNeg = deltaArr.negative.Length == 0 ? 0 :
                    -Math.Sqrt(deltaArr.negative.Sum(d => d * d / deltaArr.all.Length));
                double sumSq = deltaArr.all.Length == 0? 0 :
                    deltaArr.all.Sum(d => d * d * Math.Sign(d)) / deltaArr.all.Length;
                G = Math.Sign(sumSq) * Math.Sqrt(Math.Abs(sumSq));
               
            }
        }

        public class Deltas : Gradient.Grad
        {
            public Deltas(CourseItem[] data)
            {
                var deltaArr = new DeltaArr(data);
                GPos = deltaArr.positive.Length ==0 ? 0 : deltaArr.positive.Sum() / deltaArr.positive.Length;
                GNeg = deltaArr.negative.Length == 0 ? 0 : deltaArr.negative.Sum() / deltaArr.negative.Length;
                G = GPos + GNeg;
            }
        }


        public static double WndGrad(CourseItem[] data, double wSlope = 0.6)
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
            return g / (1 - 0.5 * wSlope);
        }

        public class DeltaArr
     {
         public double[] negative;
         public double[]  positive;
         public double[] all;
         public DeltaArr(CourseItem[] data)
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

