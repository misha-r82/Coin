using System;
using Btr;
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
            var t = api.Sell(order);
            t.Wait();
        }
    }
}
