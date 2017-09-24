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
            PlnCouse.CouseItem[] c = course.GetCouse("BTC_ETH", period, new TimeSpan(0,1,0)).ToArray();

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
