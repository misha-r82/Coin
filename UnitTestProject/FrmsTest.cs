using System;
using System.Linq;
using Btr;
using Btr.Polon;
using Btr.PrivApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class FrmsTest
    {
        [TestMethod]
        public void FrmTreaderEditorTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, new ApiParser(new ApiBase()));
            var f = new FrmTreaderEditor(treader);
            f.ShowDialog();
        }
    }
}
