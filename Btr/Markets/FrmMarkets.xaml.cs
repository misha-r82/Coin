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

namespace Btr
{
    /// <summary>
    /// Логика взаимодействия для FrmMarkets.xaml
    /// </summary>
    public partial class FrmMarkets : Window
    {
        public FrmMarkets()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddMar_Click(object sender, RoutedEventArgs e)
        {
            Markets.MarketRepo.Instance.Add(new Coin.Market(txtMarName.Text, new Coin.Polon.ApiDriver()));
        }
    }
}
