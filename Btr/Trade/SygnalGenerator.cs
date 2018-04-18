using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Btr.Trade
{
    class SygnalGenerator
    {
        public enum Sygnal { Buy, Sell, None }

        public Sygnal GetSygnal(CoursePoint point)
        {
            return Sygnal.None;
        }
    }
}
