using System;
using System.Diagnostics;
using System.Linq;
using Btr;
using Btr.Files;
using Btr.History;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void MarketLoadAndSave()
        {
            string name = "BTC_ETH";
            var market = new Market(name);
            var from = new DateTime(2017,7,15);
            var to = new DateTime(2017, 09, 11);
            market.LoadHistory(new DatePeriod(from,to));
            if (Markets.MarketList.ContainsKey(name)) Markets.MarketList.Remove(name);
            Markets.MarketList.Add(market.Name, market);
            Markets.SaveMarkets();
        }

        private void TradeTest(double T, double gap)
        {
            var m = Markets.MarketList.First();
            var tacker = new CourseTracker(m.Value, new BaseSettings()
            { Delta = 0.01, Tbase = TimeSpan.FromHours(T), T = new TimeSpan(0,0,20), GGap = gap});
            var treader = new Treader(tacker);
            foreach (PlnCouse.CouseItem item in m.Value.CourseData)
            {
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
            }
            int ptCount = m.Value.CourseData.Length;
            var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Count + treader.Sellers.Count;
            var investBtc = invest * sred;
            var marhin = treader.Complited.Sum(c => 
            c.SellPoint.Course - c.BoughtPt.Course - 0.05 * c.SellPoint.Course);
            var percent = marhin / investBtc;
            Debug.WriteLine("T ={0} gap ={1} %= {2}", T, gap, percent);
        }
        [TestMethod]
        public void TestMethod1()
        {
            for (int g = 1; g < 8; g++)
            {
                double gap = 0.1 * g;
                double t = 6;
                while (t < 130)
                {
                    TradeTest(t, gap);
                    t *= 1.5;
                }
            }
        }
    }
}
