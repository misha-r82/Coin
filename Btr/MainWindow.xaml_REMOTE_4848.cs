﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
using Btr;
using Btr.Trade;
using Coin.History;
using Coin.Polon;
using Coin.Annotations;
using Lib;
using Microsoft.Win32;

namespace Coin
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
            DbgSett.Options.Add(DbgSett.DbgOption.ApiEmulate);
            DbgSett.Options.Add(DbgSett.DbgOption.ShowBuy);
            DbgSett.Options.Add(DbgSett.DbgOption.ShowSell);
            DbgSett.Options.Add(DbgSett.DbgOption.ShowCourse);
        }
        public TradeMan TM { get; private set; }

        public Treader SelectedTreader
        {
            get { return dgTraders.SelectedItem as Treader;}
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

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
            var btn = (Button)sender;
            if (btn.Content.ToString() == "Start")
            {
                TM.StartTrade();
                btn.Content = "Stop";
            }
            else
            {
                TM.StopTreading();
                btn.Content = "Start";
            }

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
            FileIO.serializeDataContract(TM, f.FileName, new DataContractSerializerSettings(){PreserveObjectReferences = true});
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void BtnAddTreader_OnClick(object sender, RoutedEventArgs e)
        {

            Treader treader = TreaderFactory.CreateTreader;
            var f = new FrmTreaderEditor(treader);
            if (f.ShowDialog() != true) return;
            TM.Add(treader);
        }

        private void BtnBewOrder_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedTreader == null) return;
            var seller = new Seller(SelectedTreader, 0);
            var f = new FrmNewOrder(seller);
            if (f.ShowDialog()==true) SelectedTreader.Sellers.Add(f.Seller);
            
        }
    }
}
