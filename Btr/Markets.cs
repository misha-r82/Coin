using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Btr.Files;
using Lib;
using Newtonsoft.Json;

namespace Btr
{
    public class Markets
    {
        private const string DIR = "C:\\Temp\\";
        private const string FILE_EXT = "mar";
        private const string MARKETS_FILE = "markets.txt";
        public static SortedList<string,  Market> MarketList { get; }

        static Markets()
        {
            MarketList = new SortedList<string, Market>();
            try
            {
                foreach (string file in Directory.GetFiles(DIR))
                {
                    var market = MarketSerializer.DeserializeMarket(file);
                    MarketList.Add(market.Name, market);
                }
            }
            catch (Exception e)
            {

            }
        }

        public static void LoadMarkets(DatePeriod period)
        {
            foreach (var pair in MarketList)
                pair.Value.LoadHistory(period);
        }
        public static void ReloadNew()
        {
            var actions = new List<Action>();
            foreach (var pair in MarketList)
                actions.Add(pair.Value.ReloadNew);
            var pOpt = new ParallelOptions() {MaxDegreeOfParallelism = 16};
            Parallel.Invoke(pOpt, actions.ToArray());
        }
        public static void SaveMarkets()
        {
            try
            {
                foreach (var pair in MarketList)
                    MarketSerializer.SerializeMarket(pair.Value, DIR + pair.Key + '.' + FILE_EXT);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось сериализовать рынки");
            }
        }
    }
}
