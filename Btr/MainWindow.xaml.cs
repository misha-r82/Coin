﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
using Btr.Annotations;
using Btr.History;
using Btr.Polon;
using Btr.PrivApi;
using Lib;
using Microsoft.Win32;

namespace Btr
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            TM = new TradeMan();
            DataContext = this;
        }
        public TradeMan TM { get; private set; }
        //public Exchange Ex { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*Ex = new Exchange();
            ExchangeContext ec = new ExchangeContext();
            ec.ApiKey = "";
            ec.Secret = "";
            Ex.Initialise(ec);*/          
        }
        private void EditMarkets_OnClick(object sender, RoutedEventArgs e)
        {
            var treader = dgTraders.SelectedItem as Treader;
            if (treader == null) return;
            var f = new FrmTreaderEditor(treader);
            if (f.ShowDialog() != true) return;

        }
        private void btnEditTreaderClick(object sender, RoutedEventArgs e)
        {
             var send = sender as FrameworkElement;
             var treader = send.DataContext as Treader;
             if (treader == null) return;
             var f = new FrmTreaderEditor(treader);
             f.ShowDialog();
             OnPropertyChanged(nameof(TM));            
        }
        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            var tm = new TradeMan();
            tm.StartTrade();
        }

        private void BtnLoadMarkets_OnClick(object sender, RoutedEventArgs e)
        {
            var f = new OpenFileDialog();
            if (f.ShowDialog(this) != true) return;
            TM = FileIO.deserializeDataContract<TradeMan>(f.FileName);
            OnPropertyChanged(nameof(TM));            
        }

        private void BtnSaveMarkets_OnClick(object sender, RoutedEventArgs e)
        {
            var f = new SaveFileDialog();
            if (f.ShowDialog(this) != true) return;
            FileIO.serializeDataContract(TM, f.FileName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void BtnAddTreader_OnClick(object sender, RoutedEventArgs e)
        {
            var market = cbMarket.SelectedValue as Market;
            if (market == null) return;
            var tracker = new CourseTracker(market, new TrackSettings());
            var treader = new Treader(tracker, new ApiParser(new ApiBase()));
            var f = new FrmTreaderEditor(treader);
            if (f.ShowDialog() != true) return;
            TM.Add(treader);
        }
    }
}
