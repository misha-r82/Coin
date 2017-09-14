using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lib
{
    [DataContract]
    [Serializable]
    public class KVPair<TKey, TVal> : INotifyPropertyChanged, IEditableObject, ICloneable, IEquatable<KVPair<TKey, TVal>>
    {
        [DataMember]
        private TKey key;
        [DataMember]
        private TVal val;
        public KVPair(TKey key, TVal val)
        {
            this.key = key;
            this.val = val;
        }

        public KVPair()
        {
            key = default(TKey);
            val = default(TVal);
        }
        public TKey Key
        {
            get { return key; }
            set
            {
                if (key.Equals(value)) return;
                key = value;
                OnPropertyChanged("Key");
            }
        }

        public TVal Val
        {
            get { return val; }
            set
            {
                if (val.Equals(value)) return;
                val = value;
                OnPropertyChanged("Val");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        public void BeginEdit() { }
        public void CancelEdit() { }
        public void EndEdit() { }
        public object Clone()
        {
            ICloneable c = key as ICloneable;
            TKey k = key;
            if (c != null) k = (TKey) c.Clone();
            c = val as ICloneable;
            TVal v = val;
            if (c != null) v = (TVal)c.Clone();
            return new KVPair<TKey, TVal>(k, v);
        }

        public bool Equals(KVPair<TKey, TVal> other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, null)) return false;
            return other.Key.Equals(key) && other.val.Equals(val);
        }

        public int BaseHash => base.GetHashCode();

        public override int GetHashCode()
        {
            int keyCode = ReferenceEquals(key, null) ? 0 : key.GetHashCode();
            int valCode = ReferenceEquals(val, null) ? 0 : val.GetHashCode();
            return keyCode ^ valCode;
        }
    }
}
