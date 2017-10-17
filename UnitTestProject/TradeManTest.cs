using System;
using System.Threading;
using Btr;
using Btr.PrivApi;
using Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class TradeManTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var tm = new TradeMan();
            tm.StartTrade();
            while (true)
            {
                Thread.Sleep(200);
            }
        }

    }
}
