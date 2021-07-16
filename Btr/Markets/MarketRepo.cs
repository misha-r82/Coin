using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Coin;

namespace Markets
{
    [DataContract]
    public class MarketRepo : IEnumerable<Market>, INotifyCollectionChanged
    {
        [DataMember]private SortedList<string, Market> _marketList;

        public static MarketRepo Instance { get; }
        static MarketRepo()
        {
            Instance = new MarketRepo();
        }
        public MarketRepo()
        {
            _marketList = new SortedList<string, Market>();
        }

        public void Add(Market market)
        {
            if (!_marketList.ContainsKey(market.Name))
                _marketList.Add(market.Name, market);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, 
                new[] { market }));
        }
        public void Remove (string marName)
        {
            if (_marketList.ContainsKey(marName))
            {
                var market = _marketList[marName];
                _marketList.Remove(marName);
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    new[] { market }));
            }
        }






        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public IEnumerator<Market> GetEnumerator()
        {
            return _marketList.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _marketList.Values.GetEnumerator();
        }
    }
}
