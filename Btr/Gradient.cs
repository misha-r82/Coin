using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Btr.History;
using Lib;

namespace Btr
{
    public class Gradient
    {
        public static double GetGradient(PlnCouse.CouseItem[] data, DatePeriod period, TimeSpan tBase)
        {
            int count = data.Length;
            if (count == 0) return double.NaN;
            if (count == 1) return data[0].delta;
            double g = 0;
            double lastNotNull = 0;
            for (int i = 0; i < count - 1; i++)
            {
                if (data[i].course != 0) lastNotNull = data[i].course;
                if (lastNotNull > 0 && data[i + 1].course > 0)
                    g += data[i + 1].course - data[i].course;                
            }

            double kT = period.Dlit.TotalMilliseconds / tBase.TotalMilliseconds;
            return g / kT;
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
 
    }
}
