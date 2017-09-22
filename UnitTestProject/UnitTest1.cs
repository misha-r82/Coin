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

        private void TradeTest(double T, double delta ,double gap)
        {
            var m = Markets.MarketList.First();
            var tacker = new CourseTracker(m.Value, new BaseSettings()
            { Delta = delta* T / 6, T = new TimeSpan(0,0,20), Tbase = TimeSpan.FromHours(T), GGap = gap});
            var treader = new Treader(tacker);
            foreach (PlnCouse.CouseItem item in m.Value.CourseData)
            {
                /*if (item.date == new DateTime(2017,07,19,19,50,0))
                { }*/
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
            }
            int ptCount = m.Value.CourseData.Length;
            var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Count + treader.Sellers.Count;
            var investBtc = invest * sred;
            var margin = treader.Complited.Sum(c => 
            c.SellPoint.Course - c.BoughtPt.Course - 0.05 * c.SellPoint.Course);
            var percent = margin / investBtc;
            //if (percent > 0.1)
                Debug.WriteLine("T ={0} gap ={1} d ={2} %= {3}", T, gap, delta, percent);
            Debug.WriteLine("Compl ={0} List ={1}", treader.Complited.Count, treader.Sellers.Count);

        }
        [TestMethod]
        public void TradeTestCase()
        {
            TradeTest(0.02, 0.01, 0.3);
        }
        [TestMethod]
        public void TestMethod1()
        {
            double delta = 0.0005;
            while (delta < 0.025)
            {
                double gap = 0.1;
                while (gap < 0.9)
                {
                    double t = 1;
                    while (t < 130)
                    {
                        TradeTest(t, delta, gap);
                        t *= 2;
                    }
                    gap += 0.2;
                }
                delta *= 2;
            }

        }

    }
}
