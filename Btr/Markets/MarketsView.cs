using Btr.MarStat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Btr
{
    public class MarketsView
    {
        public static List<MarStatLevels> MarketList { get
            {
                var res = new List<MarStatLevels>();
                /*foreach (var market in Markets.MarketList)
                    res.Add(new MarStatLevels(market.Value));*/
                var tmp = res[0].LevelStat;
                return res;
            }
        }
    }
}
