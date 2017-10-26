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
        [TestMethod]
        public void FrmMainTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, new ApiParser(new ApiBase()));
            treader.Sellers.Add(new Seller(new Order("test_pair1",2,3), new TrackSettings() , new ApiParser(new ApiBase())));
            treader.Complited.Add(new Seller(new Order("test_pair2", 1, 5), new TrackSettings(), new ApiParser(new ApiBase())));
            var f = new MainWindow();
            f.TM.Add(treader);
            f.ShowDialog();
        }        
    }
}
