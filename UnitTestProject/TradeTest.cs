using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coin;
using Coin.Files;
using Coin.History;
using Coin.Polon;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lib;

namespace UnitTestProject
{
    [TestClass]
    public class TradeTest
    {
        private async Task TradingTest(double delta, double kGrad)
        {
            var tm = FileIO.deserializeDataContract<TradeMan>("C:\\Markt\\ltc");
            var treader = tm["PLN", "BTC_LTC"];
            treader.MinSell = 0;
            treader.MaxBuy = double.MaxValue;
            treader.Tracker.Sett = new TrackSettings()
                {Delta = delta, KGrad = kGrad};
            var period = new DatePeriod(new DateTime(2018, 04, 13, 00, 0,0), new DateTime(2018, 04,25));
            var courseData = treader.Market.GetData(period).ToArray();
            foreach (CourseItem item in courseData)
            {
                if (item.date == new DateTime(2017, 10, 19, 19, 50, 0))
                { }
                var coursePoint = new CoursePoint(item.course, item.date);
                await treader.Trade(coursePoint);
                //Debug.WriteLine("{0}|{1}", coursePoint, item.delta);
            }
            int ptCount = courseData.Length;
            //var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Sum(o => o.BuyOrder.Amount);
            var margin = treader.Complited.Sum(c =>
                (c.SellOrder.Price - c.BuyOrder.Price - 0.005 * c.SellOrder.Price) / c.BuyOrder.Price);
            var percent = margin;
            if (treader.Complited.Count > -1 && percent > -1)
            {
                Debug.WriteLine("kGrad ={0} d ={1} %= {2}", kGrad, delta, percent);
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
            var task = Task.Factory.StartNew(() => TradingTest(0.01, 10));
            task.Wait();

        }
        /*private IApiDriver Market => new Coin.Polon.ApiDriver(new Coin.Polon.ApiWeb());
        private void TradeTest(double delta, double kGrad)
        {
            var m = Markets.MarketList["USDT_BTC"];
            var tacker = new CourseTracker(m, new TrackSettings()
            { Delta = delta, KGrad = kGrad });
            var treader = new Treader(tacker, Market);
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
                Debug.WriteLine("kGrad ={0} d ={1} %= {2}", kGrad, delta, percent);
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
                double kGrad = 0;
                while (kGrad < 1.1)
                {
                    TradeTest(delta, kGrad);
                    kGrad *= 1.5;
                }
                delta += 0.005;
            }

        }*/

    }
}
