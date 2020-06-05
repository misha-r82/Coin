using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Btr.MarStat;
using Lib;

namespace Coin
{
    /// <summary>
    /// Interaction logic for FrmAddMarket.xaml
    /// </summary>
    public partial class FrmMarketList : Window
    {
        public FrmMarketList()
        {
            InitializeComponent();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            string name = txtMarketName.Text;
            var market = new Market(name);
            market.LoadHistory(new DatePeriod(DateTime.Now - MultiPeriodGrad.MaxPeriod, DateTime.Now));
            Markets.MarketList.Add(name, market);
            lvMarkets.Items.Refresh();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            Markets.SaveMarkets();
        }

        private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            var markets = lvMarkets.SelectedItems.OfType<Market>();
            foreach (var market in markets)
                Markets.MarketList.Remove(market.Name);
            lvMarkets.Items.Refresh();
        }

        private void btnLoad_OnClick(object sender, RoutedEventArgs e)
        {
            var mStat = ((FrameworkElement) sender).DataContext as MStatBase;
            if (mStat == null) return;
            mStat.Market.LoadHistory();
            lvMarkets.Items.Refresh();
        }

    }
}
