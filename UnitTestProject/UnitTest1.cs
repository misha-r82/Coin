using System;
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

        [TestMethod]
        public void TestMethod1()
        {
            var m = Markets.MarketList.First();
            var tacker = new CourseTracker(m.Value, new BaseSettings()
            { Delta = 0.01, T = new TimeSpan(0, 12, 0, 0) });
            var treader = new Treader(tacker);
            foreach (PlnCouse.CouseItem item in m.Value.CourseData)
            {
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
            }
        }
    }
}
