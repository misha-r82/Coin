using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Btr.Trade
{
    class SmartDelta
    {
        public static double GetPosDelta(Gradient.Grad grad, double delta)
        {
            double smartDelta = Math.Abs(grad.GPos / grad.GNeg) * delta;
            if (smartDelta < delta) smartDelta = delta;
            return smartDelta;
        }
        public static double GetNegDelta(Gradient.Grad grad, double delta)
        {
            double smartDelta = Math.Abs(grad.GNeg / grad.GPos) * delta;
            if (smartDelta < delta) smartDelta = delta;
            return smartDelta;
        }
    }
}
