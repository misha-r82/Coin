using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Lib.Annotations;
using Xceed.Wpf.Toolkit;

namespace Lib
{
    /// <summary>
    /// Interaction logic for PeriodControl.xaml
    /// </summary>
    public partial class PeriodControl : UserControl
    {
        public PeriodControl()
        {
            InitializeComponent();
            ResetDateTime(DefPeriod, new TimeSpan(0));
            //GridDateTime.DataContext = Period;
        }
        /*public DatePeriod Period
        {
            get { return (DatePeriod)GetValue(PeriodProperty); }
            set { SetValue(PeriodProperty, value); }
        }

        public static readonly DependencyProperty PeriodProperty =
            DependencyProperty.Register("Period", typeof(DatePeriod), typeof(PeriodControl), 
                new FrameworkPropertyMetadata(DefPeriod) {BindsTwoWayByDefault = true});*/

        public static DatePeriod DefPeriod { get { return ResetDateTime(new DatePeriod(), new TimeSpan(0)); } }
        private static DatePeriod ResetDateTime(DatePeriod sourcePeriod, TimeSpan resetPeriod)
        {
            DateTime dta = DateTime.Today;
            DateTime dtaFrom = dta.Date + resetPeriod;
            DateTime dtaTo = dta.Date + new TimeSpan(0, 23, 59, 59);
            sourcePeriod.From = dtaFrom;
            sourcePeriod.To = dtaTo;
            return sourcePeriod;
        }
        private void resetClick(object sender, TimeSpan resetPeriod)
        {
            var btn = sender as SplitButton;
            if (btn != null) btn.IsOpen = false;
            var period = ((FrameworkElement)sender).DataContext as DatePeriod;
            if (period == null) return;
            ResetDateTime(period, resetPeriod);
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            { resetClick(sender, new TimeSpan(-7, 0, 0, 0)); return; }
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            { resetClick(sender, new TimeSpan(-31, 0, 0, 0)); return; }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            { resetClick(sender, new TimeSpan(-61, 0, 0, 0)); return; }
            resetClick(sender, new TimeSpan(0));
        }

        private void ResetItm1_OnClick(object sender, RoutedEventArgs e)
        {
            resetClick(sender, new TimeSpan(-7, 0, 0, 0)); btnReset.IsOpen = false;
            //Debug.WriteLine("Ctrl period {0}", Period);
        }
        private void ResetItm2_OnClick(object sender, RoutedEventArgs e)
        { resetClick(sender, new TimeSpan(-31, 0, 0, 0)); btnReset.IsOpen = false; }
        private void ResetItm3_OnClick(object sender, RoutedEventArgs e)
        { resetClick(sender, new TimeSpan(-60, 0, 0, 0)); btnReset.IsOpen = false; }
        private void ResetItm4_OnClick(object sender, RoutedEventArgs e)
        { resetClick(sender, new TimeSpan(-90, 0, 0, 0)); btnReset.IsOpen = false; }
        private void ResetItm5_OnClick(object sender, RoutedEventArgs e)
        { resetClick(sender, new TimeSpan(-180, 0, 0, 0)); btnReset.IsOpen = false; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
