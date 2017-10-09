using System;
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
    }
}
