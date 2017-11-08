using System;
using System.Linq;
using Btr;
using Btr.History;
using Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class HistoryTest
    {
        private TimeSpan _interval = new TimeSpan(0, 0, 5);
        [TestMethod]
        public void TestMethod1()
        {
            DateTime from = DateTime.Now - new TimeSpan(3,0,0);
            var cc = new Cryptocompare();
            var data = cc.GetCourse("USDT", "ZEC", new DatePeriod(from, DateTime.Now)).ToArray();
            var max = data.Max(d => d.date);
        }
        [TestMethod]
        public void TestHistoryPln()
        {
            DateTime to = DateTime.Now;
            DateTime from = to - new TimeSpan(3,0,0);
            var data = PlnHistory.GetHitoryPln("USDT_ZEC", new DatePeriod(from, to)).ToArray();
            var max = data.Max(d => d.date);
            var min = data.Min(d => d.date);
            
        }
        [TestMethod]
        public void TestHistoryPln1()
        {
            DateTime to = DateTime.Now;
            DateTime from = to - new TimeSpan(3, 0, 0);
            var course = new PlnCouse();
            var data = course.GetHistory("USDT_ZEC", new DatePeriod(from, to), _interval).ToArray();
            var max = data.Max(d => d.date);
            var min = data.Min(d => d.date);

        }
        [TestMethod]
        public void TestPlnCourse()
        {
            DateTime to = DateTime.Now - new TimeSpan(3,20,0);
            DateTime from = to - new TimeSpan(0, 3, 0);
            var market = new Market("USDT_ZEC");
            market.LoadHistory(new DatePeriod(from, to));
            var max = market.CourseData.Max(d => d.date);
            var min = market.CourseData.Min(d => d.date);
            market.LoadHistory();
            max = market.CourseData.Max(d => d.date);
            min = market.CourseData.Min(d => d.date);

        }
    }
}
