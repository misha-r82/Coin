using System;
using System.Threading.Tasks;
using Coin;
using Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ApiTestPln
    {
        private ApiWebBase ApiWeb => new Coin.Polon.ApiWeb();
        [TestMethod]
        public void TestSell()
        {
            var order = new Order("BTC_ETH", 1, 0.01);
            var parser = new Coin.Polon.ApiDriver();
            var t = parser.Sell(order);
            t.Wait();
            t = parser.IsComplited(order);
            t.Wait();
            t = parser.CanselOrder(order);
            t.Wait();
        }
        [TestMethod]
        public void TestHistory()
        {
            var parser = new Coin.Polon.ApiDriver();
            Task<Order[]> t =  parser.OrderHistory("BTC_ZEC", new DatePeriod(new DateTime(2018, 01, 01), DateTime.Now));
            Task.WaitAll(t);
            Order[] res = t.Result;
        }
        [TestMethod]
        public void TestOrderList()
        {
            /*var api = new Coin.Polon.ApiDriver(new Coin.Polon.ApiWeb());
            var parser = ApiWeb;
            var period = new DatePeriod(
                new DateTime(2017, 10, 5, 20, 30, 0), 
                new DateTime(2017, 10, 5, 20, 32, 0));
            //var t = parser.OrderHistory("BTC_ZEC", period);
            //t.Wait();*/
        }
    }
}
