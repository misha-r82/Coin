using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;
using Lib.Annotations;

namespace Lib
{
    //    public class SortList<T> : IList<T>, IList where T : class
    //    {
    //        private List<T> list; 
    //        public SortList()
    //        {
    //            baseList = new SortedList<string, T>();
    //            list = new List<T>();
    //        }
    //        private SortedList<string, T> baseList;
    //        public IEnumerator<T> GetEnumerator()
    //        {
    //            return baseList.Values.GetEnumerator();
    //        }
    //        IEnumerator IEnumerable.GetEnumerator()
    //        {
    //            return GetEnumerator();
    //        }
    //#region INSERT
    //        public void Add(T item)
    //        {
    //            string key = item.ToString();
    //            if (baseList.ContainsKey(key)) return;
    //            baseList.Add(key,item);
    //            list.Add(item);
    //            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));            
    //        }
    //        public int Add(object value)
    //        {
    //            var item = value as T;
    //            if (item != null) Add(item);
    //            //return baseList.Count - 1;
    //            return list.Add(va);
    //        }
    //        public void Insert(int index, object value)
    //        {
    //            throw new NotImplementedException();
    //        }
    //        public void Insert(int index, T item)
    //        {
    //            throw new NotImplementedException();
    //        }
    //#endregion
    //#region DELETE
    //        public void Remove(object value)
    //        {
    //            var item = value as T;
    //            if (item == null) return;
    //            Remove(item);
    //        }
    //        public void Clear()
    //        {
    //            baseList.Clear();
    //            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    //        }
    //        public bool Remove(T item)
    //        {
    //            bool rez = baseList.Remove(item.ToString());
    //            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
    //            return rez;
    //        }
    //        public void RemoveAt(int index)
    //        {
    //            var item = baseList[index.ToString()];
    //            baseList.RemoveAt(index);
    //            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
    //        }
    //#endregion
    //#region UPDATE
    //        object IList.this[int index]
    //        {
    //            get { return list[index]; }
    //            set
    //            {
    //                var item = value as T;
    //                if (item == null) return;
    //                baseList.Values[index] = item;
    //            }
    //        }
    //        public T this[int index]
    //        {
    //            get { return list[index]; }
    //            set
    //            {
    //                baseList.Values[index] = value;
    //                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value));
    //            }
    //        }
    //        public T this[string key]
    //        {
    //            get { return baseList[key]; }
    //            set
    //            {
    //                var item = baseList[key];
    //                baseList[key] = value;
    //                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item));
    //            }
    //        }
    //#endregion
    //        public bool Contains(object value)
    //        {
    //            var item = value as T;
    //            if (item == null) return false;
    //            return Contains(item);
    //        }
    //
    //        public int IndexOf(object value)
    //        {
    //            var item = value as T;
    //            if (item == null) return -1;
    //            return IndexOf(item);
    //        }
    //        public bool Contains(T item)
    //        {
    //            return baseList.ContainsKey(item.ToString());
    //        }
    //        public void CopyTo(T[] array, int arrayIndex)
    //        {
    //            throw new NotImplementedException();
    //        }
    //        public void CopyTo(Array array, int index)
    //        {
    //            throw new NotImplementedException();
    //        }
    //        public int Count { get { return baseList.Count; } }
    //        public object SyncRoot { get; }
    //        public bool IsSynchronized { get { return false;} }
    //        public bool IsReadOnly { get { return false; } }
    //        public bool IsFixedSize { get { return false; } }
    //        public int IndexOf(T item)
    //        {
    //            return baseList.IndexOfKey(item.ToString());
    //        }
    //
    //        public delegate void PreviewAddHandler(object sender, ref T newItem);
    //        public event NotifyCollectionChangedEventHandler CollectionChanged;
    //        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    //        {
    //            if (CollectionChanged==null) return;
    //            CollectionChanged.Invoke(this, args);
    //        }
    //
    //    }

}
