using System;
using System.Linq;
using Coin;
using Coin.Polon;
using Coin.Btr;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    
    [TestClass]
    public class FrmsTest
    {
        private IApiDriver Api => new Coin.Polon.ApiDriver(new Coin.Polon.ApiWeb());
        [TestMethod]
        public void FrmTreaderEditorTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, Api);
            var f = new FrmTreaderEditor(treader);
            f.ShowDialog();
        }
        [TestMethod]
        public void FrmMainTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, Api);
            treader.Sellers.Add(new Seller(new Order("test_pair1",2,3), new TrackSettings() ,Api));
            var complSeller = new Seller(new Order("test_pair2", 1, 5), new TrackSettings(), Api);
            treader.Complited.Add(complSeller);
            var f = new MainWindow();
            f.TM.Add(treader);
            f.ShowDialog();
        }        
    }
}
