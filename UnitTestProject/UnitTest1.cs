using System;
using System.Diagnostics;
using System.Linq;
using Coin;
using Coin.Files;
using Coin.History;
using Coin.Polon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        private IApiDriver Api => new Coin.Polon.ApiDriver(new Coin.Polon.ApiWeb());
        [TestMethod]
        public void MarketLoadAndSave()
        {
            string name = "USDT_BTC";
            var market = new Market(name);
            var from = new DateTime(2017,09,1);
            var to = new DateTime(2017, 10, 31);
            market.LoadHistory(new DatePeriod(from,to));
            if (Markets.MarketList.ContainsKey(name)) Markets.MarketList.Remove(name);
            Markets.MarketList.Add(market.Name, market);
            Markets.SaveMarkets();
        }

        private void TradeTest(double delta, double gap)
        {
            var m = Markets.MarketList["USDT_BTC"];
            var tacker = new CourseTracker(m, new TrackSettings()
            { Delta = delta, GGap = gap });
            var treader = new Treader(tacker, Api);
            var period = new DatePeriod(new DateTime(2017,10,1), new DateTime(2017,10,30));
            var courseData = m.GetData(period);
            foreach (CourseItem item in courseData)
            {
                if (item.date == new DateTime(2017,10,19,19,50,0))
                { }
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
                //Debug.WriteLine("{0}|{1}", coursePoint, item.delta);
            }
            int ptCount = m.CourseData.Length;
            //var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Sum(o=>o.BuyOrder.Amount);
            var margin = treader.Complited.Sum(c => 
            (c.SellOrder.Price - c.BuyOrder.Price - 0.005 * c.SellOrder.Price)/ c.BuyOrder.Price);
            var percent = margin;
            if (treader.Complited.Count >-1 && percent > -1)
            {
                Debug.WriteLine("gap ={0} d ={1} %= {2}", gap, delta, percent);
                Debug.WriteLine("Compl ={0} List ={1}", treader.Complited.Count, treader.Sellers.Count);                
            }
        }
        [TestMethod]
        public void TradeTestCase()
        {
            DbgSett.Options.Clear();
            DbgSett.Options.Add(DbgSett.DbgOption.ShowBuy);
            DbgSett.Options.Add(DbgSett.DbgOption.ShowSell);
            DbgSett.Options.Add(DbgSett.DbgOption.ApiEmulate);
            //DbgSett.Options.Add(DbgSett.DbgOption.ShowCourse);
            TradeTest(0.007, 0.7);
            
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
                    TradeTest(delta, gap);
                    gap *= 1.5;
                }
                delta += 0.005;
            }

        }

    }
}
