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
        /*private IApiDriver Market => new Coin.Polon.ApiDriver(new Coin.Polon.ApiWeb());
        [TestMethod]
        public void FrmTreaderEditorTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, Market);
            var f = new FrmTreaderEditor(treader);
            f.ShowDialog();
        }
        [TestMethod]
        public void FrmMainTest()
        {
            var tracker = new CourseTracker(Markets.MarketList.First().Value, new TrackSettings());
            var treader = new Treader(tracker, Market);
            treader.Sellers.Add(new Seller(new Order("test_pair1",2,3), new TrackSettings() ,Market));
            var complSeller = new Seller(new Order("test_pair2", 1, 5), new TrackSettings(), Market);
            treader.Complited.Add(complSeller);
            var f = new MainWindow();
            f.TM.Add(treader);
            f.ShowDialog();
        }   */     
    }
}
