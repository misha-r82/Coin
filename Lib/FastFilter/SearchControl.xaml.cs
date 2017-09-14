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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lib.QuickFilter;

namespace Lib
{
    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl : UserControl
    {
        private IControlQFilter fltControl; 
        public SearchControl()
        {
            InitializeComponent();           
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            fltControl = DataContext as IControlQFilter;
            // ReSharper disable once UseNullPropagation
            if (fltControl != null && fltControl.FilterAction != null)
                fltControl.FilterAction();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var dbg = lstCols.DataContext;
            fltControl = DataContext as IControlQFilter;
        }

        private void TxtSearch_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
                fltControl.FilterAction();
        }

        private void btnCols_Click(object sender, RoutedEventArgs e)
        {
            rowColumns.Height = btnCols.IsChecked == true ? new GridLength(1, GridUnitType.Auto) : new GridLength(0);
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            fltControl.SetFiltersEn(chkSelectAll.IsChecked == true);
        }
    }
}
