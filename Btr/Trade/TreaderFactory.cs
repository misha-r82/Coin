using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Btr.Trade
{
    public class TreaderFactory
    {
        static TreaderFactory()
        {
            Api = new Coin.Polon.ApiDriver();
            TrackSett = new TrackSettings();
        }
        public static IApiDriver Api { get; }
        public static TrackSettings TrackSett { get; }
        public static string MarketName { get; set; }

        public static Treader CreateTreader
        {
            get
            {
                var market = new Market(MarketName, Api);
                var treader = new Treader(market, TrackSett);
                return treader;
            }
        }
    }
}
