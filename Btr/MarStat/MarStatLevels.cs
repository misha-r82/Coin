using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Btr.MarStat
{
    class LevelN
    {
        public double C0, K;
        public LevelN(double c0, double k)
        {
            C0 = c0;
            K = k;
        }

        public double C(int n)
        {
            if (n < 0) return C0 * Math.Pow(1 - K, -n);
            return C0 * Math.Pow(1 + K, n);
        }
    }

    public class MarStatLevels : MarStat.MStatBase
    {
        public MarStatLevels(Market market) : base(market)
        {
        }
        public SortedList<int, int> LevelStat
        {
            get
            {
                var res = new SortedList<int, int>();
                var lev = new LevelN(Market.CourseData[0].course, 0.005);
                int n = 0;
                foreach (CourseItem courseItem in Market.CourseData)
                {
                    if (courseItem.course == 0) continue;
                    while (courseItem.course > lev.C(n+2))
                    {
                        n += 2;
                        if (!res.ContainsKey(n)) res.Add(n, 1);
                        else res[n]++;
                    }
                    while (courseItem.course < lev.C(n - 2))
                    {
                        n -= 2;
                        if (!res.ContainsKey(n)) res.Add(n, 1);
                        else res[n]++;
                    }
                }
                return res;
            }
        }
    }
}
