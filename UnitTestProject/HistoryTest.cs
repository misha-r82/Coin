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
        [TestMethod]
        public void TestMethod1()
        {
            DateTime from = DateTime.Now - new TimeSpan(3,0,0);
            var cc = new Cryptocompare();
            var data = cc.GetCourse("USDT", "ZEC", new DatePeriod(from, DateTime.Now)).ToArray();
            var max = data.Max(d => d.date);
        }
    }
}
