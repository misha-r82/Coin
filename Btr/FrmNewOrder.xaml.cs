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
using Coin;

namespace Btr
{
    /// <summary>
    /// Логика взаимодействия для FrmNewOrder.xaml
    /// </summary>
    public partial class FrmNewOrder : Window
    {
        public Seller Seller { get; }
        public FrmNewOrder(Seller seller)
        {
            Seller = seller;
            InitializeComponent();
            DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true; 
        }
    }
}
