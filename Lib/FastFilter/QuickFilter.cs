using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Test.Annotations;

namespace Lib.QuickFilter
{
    public interface IControlQFilter
    {
        Action FilterAction { get; }
        string FilterStr { get; set; }

        void SetFiltersEn(bool en);
    }
    public class QuickFilter<T> : IControlQFilter, INotifyPropertyChanged
    {
        public List<FieldFilter> Filters { get; set; }
        private T[] _resultCollection;
        private IEnumerable<T> _sourceCollection;
        public Action FilterAction { get { return OnSerchAction; } }
        public QuickFilter()
        {
            Filters = new List<FieldFilter>();
        }
        public string FilterStr { get; set; }
        public void SetFiltersEn(bool en)
        {
            foreach (var flt in Filters)
                flt.Enabled = en;
        }

        public T[] ResultCollection
        {
            get { return _resultCollection; }
            set { _resultCollection = value; }
        }
        public IEnumerable<T> SourceCollection
        { get { return _sourceCollection; }
            set
            {
                _sourceCollection = value;
                OnPropertyChanged(nameof(SourceCollection));
                RunFilter();
            }
        }
        
        private void RunFilter()
        {
            if (_sourceCollection == null) return;
            if (string.IsNullOrEmpty(FilterStr)) _resultCollection = _sourceCollection.ToArray();
            else _resultCollection = _sourceCollection.Where(PassFilters).ToArray();
            OnPropertyChanged(nameof(ResultCollection));
        }
        private bool PassFilters(T item)
        {
            foreach (var filter in Filters)
                if (filter.Pass(item, FilterStr)) return true;
            return false;
        }

        public void AddFilter(string name, Func<T, string> fieldSelector)
        {
            Filters.Add(new FieldFilter(name, fieldSelector));
        }
        public class FieldFilter : INotifyPropertyChanged
        {
            public FieldFilter(string name, Func<T, string> fieldSelector)
            {
                Enabled = true;
                Name = name;
                FieldSelector = fieldSelector;
            }

            public bool Enabled
            {
                get { return _enabled; }
                set
                {
                    _enabled = value;
                    OnPropertyChanged(nameof(Enabled));
                }
            }

            public string Name { get; set; }
            public Func<T, string> FieldSelector;
            private bool _enabled;

            public bool Pass(T item, string patt)
            {
                if (!Enabled) return false;
                return FieldSelector(item).ToLower().Contains(patt.ToLower());
            }

            public event PropertyChangedEventHandler PropertyChanged;

            [NotifyPropertyChangedInvocator]
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event Action RefiterNeeded;
        protected virtual void OnSerchAction()
        {
            RunFilter();
            if (RefiterNeeded != null) RefiterNeeded();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
