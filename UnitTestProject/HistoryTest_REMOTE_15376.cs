﻿using System;
using System.Linq;
using System.Net.NetworkInformation;
using Coin;
using Coin.Api;
using Coin.Data;
using Coin.History;
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
            var apiCall = new ApiCall(false);
            apiCall.CallWithJsonResponse<HistoryItem[]>(
                @"https://poloniex.com/exchange#btc_eth");
        }
        /*private TimeSpan _interval = new TimeSpan(0, 0, 5);
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
            var course = new Course();
            var data = course.GetHistory("USDT_ZEC", new DatePeriod(from, to), _interval).ToArray();
            var max = data.Max(d => d.date);
            var min = data.Min(d => d.date);

        }
        [TestMethod]
        public void TestPlnCourse()
        {
            DateTime to = DateTime.Now - new TimeSpan(3,40,0);
            DateTime from = to - new TimeSpan(0, 20, 0);
            var market = new Market("USDT_ZEC");
            market.LoadHistory(new DatePeriod(from, to));
            var max = market.CourseData.Max(d => d.date);
            var min = market.CourseData.Min(d => d.date);
            market.LoadHistory();
            max = market.CourseData.Max(d => d.date);
            min = market.CourseData.Min(d => d.date);

        }*/
    }
}
