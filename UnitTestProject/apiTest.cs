using System;
using Btr;
using Btr.PrivApi;
using Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ApiTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var api = new Btr.Polon.ApiBase();
            var t = api.MyFunc();
            t.Wait();
        }
        [TestMethod]
        public void TestSell()
        {
            var api = new Btr.Polon.ApiBase();
            var order = new Order("BTC_LBC", 1, 1);
            var parser = new ApiParser(api);
            var t = parser.Sell(order);
            t.Wait();
        }
        [TestMethod]
        public void TestHistory()
        {
            var api = new Btr.Polon.ApiBase();
            var parser = new ApiParser(api);
            var period = new DatePeriod(
                new DateTime(2017, 10, 5, 20, 30, 0), 
                new DateTime(2017, 10, 5, 20, 32, 0));
            var t = parser.OrderHistory("BTC_ZEC", period);
            t.Wait();
        }
    }
}
