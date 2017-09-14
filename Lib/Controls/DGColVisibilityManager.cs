using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Test.Annotations;

namespace Lib
{
    public class DgColVisibilityManager 
    {
        private DataGrid dg;
        public DgColumn[] ColsVis { get; set; }
        public DgColVisibilityManager(DataGrid dg)
        {
            this.dg = dg;
            int cols = dg.Columns.Count;
            ColsVis = new DgColumn[cols];
            int pos = 0;
            foreach (DataGridColumn col in dg.Columns)
            {
                ColsVis[pos] = new DgColumn(col.Header.ToString());
                ColsVis[pos].PropertyChanged += (s, a) => OnColsChanged();
                pos++;
            }
                
        }

        public void UpdateGrid(DataGrid dg)
        {
            for (int i = 0; i < dg.Columns.Count; i++)
                dg.Columns[i].Visibility = ColsVis[i].Visibility;
        }
        public class DgColumn : INotifyPropertyChanged
        {
            public DgColumn(string header)
            {
                Header = header;
                IsVisible = true;
                Show = true;
            }
            public string Header { get; set; }
            private bool show;
            private bool isVisible;

            public Visibility Visibility
            {
                get { return IsVisible ? Visibility.Visible : Visibility.Collapsed; }
            }

            public bool IsVisible
            {
                get { return isVisible && show; }
                set
                {
                    if (value == isVisible) return;
                    isVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Visibility));
                }
            }
            public bool Show
            {
                get { return show; }
                set
                {
                    if (value == show) return;
                    show = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Visibility));
                }
            }
            



            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [field: NonSerialized]
        public event Action ColsChanged;
        protected virtual void OnColsChanged()
        {
            UpdateGrid(dg);
            ColsChanged?.Invoke();
        }

    }
}
