using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Bittrex;
using Btr.History;
using Lib;

namespace Btr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public Exchange Ex { get; set; }
        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            //var t = Ex.GetBalances();
            //var t = Ex.GetMarketHistory("BTC-BCC");
            var from = new DateTime(2017,8,1);
            var to = new DateTime(2017, 8, 2);
            var period = new DatePeriod(from, to);
            //var t0 = BtrHistory.GetHitoryBtr("BTC-ETH", from);
            //var t = BtrHistory.GetHitoryPln("BTC_ETH", from, to);
            var course = new PlnCouse();
            PlnCouse.CouseItem[] c = course.GetHistory("BTC_ETH", period, new TimeSpan(0,1,0)).ToArray();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*Ex = new Exchange();
            ExchangeContext ec = new ExchangeContext();
            ec.ApiKey = "";
            ec.Secret = "";
            Ex.Initialise(ec);*/
            

        }
        private void TradeTest(double T, double delta, double gap)
        {
            var m = Markets.MarketList.First();
            var tacker = new CourseTracker(m.Value, new BaseSettings()
            { Delta = delta, GGap = gap });
            var treader = new Treader(tacker);
            foreach (PlnCouse.CouseItem item in m.Value.CourseData)
            {
                /*if (item.date == new DateTime(2017,07,19,19,50,0))
                { }*/
                var coursePoint = new CoursePoint(item.course, item.date);
                treader.Trade(coursePoint);
            }
            int ptCount = m.Value.CourseData.Length;
            var sred = m.Value.CourseData.Sum(p => p.course / ptCount);
            var invest = treader.Complited.Count + treader.Sellers.Count;
            var investBtc = invest * sred;
            var margin = treader.Complited.Sum(c =>
            c.SellPoint.Course - c.BoughtPt.Course - 0.05 * c.SellPoint.Course);
            var percent = margin / investBtc;
            //if (percent > 0.1)
            Debug.WriteLine("T1 ={0} gap ={1} d ={2} %= {3}", T, gap, delta, percent);
            Debug.WriteLine("Compl ={0} List ={1}", treader.Complited.Count, treader.Sellers.Count);

        }
        public void TradeTestCase()
        {
            TradeTest(0.02, 0.01, 0.3);
        }

        public void TestMethod1()
        {
            double delta = 0.0005;
            while (delta < 0.025)
            {
                double gap = 0.1;
                while (gap < 0.9)
                {
                    double t = 1;
                    while (t < 130)
                    {
                        TradeTest(t, delta, gap);
                        t *= 2;
                    }
                    gap += 0.2;
                }
                delta *= 2;
            }

        }
        private void EditMarkets_OnClick(object sender, RoutedEventArgs e)
        {
            var f = new FrmAddMarket();
            f.ShowDialog();
        }

        private void BtnFindLeap_OnClick(object sender, RoutedEventArgs e)
        {
            DateTime start = new DateTime(2017,09,10,10,0,0);
            TestMethod1();
            /*var lf = new LeapFounder();
            lf.Market = Markets.MarketList["BTC_ETH"];
            lf.GRatio = 1.3;
            lf.TRatio = 3;
            lf.MaxPeriod = new TimeSpan(5,0,0,0);
            var leap = lf.FindLeap(start, new TimeSpan(1, 0, 0));*/
        }
    }
}
