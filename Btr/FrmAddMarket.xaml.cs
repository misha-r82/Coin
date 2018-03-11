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
using Lib;

namespace Coin
{
    /// <summary>
    /// Interaction logic for FrmAddMarket.xaml
    /// </summary>
    public partial class FrmAddMarket : Window
    {
        public FrmAddMarket()
        {
            InitializeComponent();
        }

        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            string name = txtMarketName.Text;
            var market = new Market(name, new Polon.ApiDriver());
            market.LoadHistory(new DatePeriod(DateTime.Now - MultiPeriodGrad.MaxPeriod, DateTime.Now));
            //Markets.MarketList.Add(market);
            //lvMarkets.Items.Refresh();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            //Markets.SaveMarkets();
        }

        private void BtnRemove_OnClick(object sender, RoutedEventArgs e)
        {
            //var markets = lvMarkets.SelectedItems.OfType<Market>();
            //foreach (var market in markets)
                //Markets.MarketList.Remove(market);
            //lvMarkets.Items.Refresh();
        }
    }
}
