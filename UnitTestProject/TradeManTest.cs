﻿using System;
using System.Threading;
using System.Windows;
using Coin;

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
            MessageBox.Show("123");
        }

    }
}
