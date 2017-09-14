using System;
using System.Windows;


namespace Lib
{
    /// <summary>
    /// Interaction logic for FrmStatus.xaml
    /// </summary>
    public partial class FrmStatus : Window
    {
        public FrmStatus(StatusIndicator statusIndicator)
        {
            InitializeComponent();
            this.statusIndicator = statusIndicator;
            
        }
        private StatusIndicator statusIndicator;

        public StatusIndicator StatusIndicator
        {
            get { return statusIndicator; }
            set { statusIndicator = value; }
        }

        public FrmStatus()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mainGrid.DataContext = StatusIndicator;

        }

        private void Window_Activated(object sender, EventArgs e)
        {
        }

        private void btnCansel_Click(object sender, RoutedEventArgs e)
        {
            statusIndicator.Status = StatusIndicator.StatusProc.IsCancel;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            statusIndicator.Status = StatusIndicator.StatusProc.IsCancel;
        }
    }
}
