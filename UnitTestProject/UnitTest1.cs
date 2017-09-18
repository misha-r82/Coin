using System;
using Btr;
using Btr.Files;
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

        }
    }
}
