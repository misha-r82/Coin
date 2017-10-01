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
            var from = new DateTime(2017,08,01);
            var to = new DateTime(2017, 09, 23);
            market.LoadHistory(new DatePeriod(from,to));
            if (Markets.MarketList.ContainsKey(name)) Markets.MarketList.Remove(name);
            Markets.MarketList.Add(market.Name, market);
            Markets.SaveMarkets();
        }

        private void TradeTest(double T, double delta ,double gap)
        {
            var m = Markets.MarketList.First();
            var t0 = TimeSpan.FromHours(T);
            var tacker = new CourseTracker(m.Value, new BaseSettings()
            { Delta = delta, GGap = gap});
            var treader = new Treader(tacker);
            var period = new DatePeriod(new DateTime(2017,09,4), new DateTime(2017,9,5));
            var courseData = m.Value.GetData(period);
            foreach (PlnCouse.CouseItem item in courseData)
            {
                /*if (item.date == new DateTime(2017,07,19,19,50,0))
                { }*/
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
            }
            int ptCount = m.Value.CourseData.Length;
            //var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Sum(o=>o.BoughtPt.Course)/* + treader.Sellers.Count*/;
            var margin = treader.Complited.Sum(c => 
            c.SellPoint.Course - c.BoughtPt.Course - 0.005 * c.SellPoint.Course);
            var percent = margin / invest;
            if (treader.Complited.Count >-1 && percent > -1)
            {
                Debug.WriteLine("T1 ={0} gap ={1} d ={2} %= {3}", T, gap, delta, percent);
                Debug.WriteLine("Compl ={0} List ={1}", treader.Complited.Count, treader.Sellers.Count);                
            }


        }
        [TestMethod]
        public void TradeTestCase()
        {
            DbgSett.Options.Clear();
            DbgSett.Options.Add(DbgSett.DbgOption.ShowBuy);
            DbgSett.Options.Add(DbgSett.DbgOption.ShowSell);
            //DbgSett.Options.Add(DbgSett.DbgOption.ShowCourse);
            TradeTest(0.2, 0.007, 0.3);
        }
        [TestMethod]
        public void TestMethod1()
        {
            double delta = 0.005;
            while (delta < 0.3)
            {
                double gap = 0;
                while (gap < 1.1)
                {
                    double t = 2;
                    while (t < 130)
                    {
                        TradeTest(t, delta, gap);
                        t *= 2;
                    }
                    gap *= 1.5;
                }
                delta += 0.005;
            }

        }

    }
}
