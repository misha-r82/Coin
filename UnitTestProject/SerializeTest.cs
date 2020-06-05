using System;
using System.Threading;
using System.Windows;
using Btr.Trade;
using Coin;
using Coin.Files;
using Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class SerializeTeest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var tm = new TradeMan();
            TreaderFactory.MarketName = "BTC_ETH";
            var treader = TreaderFactory.CreateTreader;
            tm.Add(treader);
            treader.Market.LoadHistory();
            //FileIO.serializeDataContract(tm, @"c:\Markt\tm.tm");
            var tm1 = FileIO.deserializeDataContract<TradeMan>(@"c:\Markt\tm.tm");

        }
        
    }
}
