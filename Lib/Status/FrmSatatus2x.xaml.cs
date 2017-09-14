using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Lib;

namespace YAK_DB_Config
{
    /// <summary>
    /// Interaction logic for FrmSatatus2x.xaml
    /// </summary>
    public partial class FrmSatatus2x : Window
    {
        public FrmSatatus2x()
        {
            InitializeComponent();
        }

        private StatusIndicator status1;
        private StatusIndicator status2;

        public StatusIndicator Status1
        {
            get { return status1; }
            set
            {
                status1 = value;
                mainGrid1.DataContext = status1;
            }
        }

        public StatusIndicator Status2
        {
            get { return status2; }
            set
            {
                status2 = value;
                mainGrid2.DataContext = status2;
            }
        }

        private void btnCansel1_Click(object sender, RoutedEventArgs e)
        {
            status1.Status = StatusIndicator.StatusProc.IsCancel;
        }
        private void btnCansel2_Click(object sender, RoutedEventArgs e)
        {
            status2.Status = StatusIndicator.StatusProc.IsCancel;
        }


    }
}
