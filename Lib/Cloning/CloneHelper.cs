using System;
using System.Collections.Generic;
using Lib;

namespace Lib
{
    public static class CloneHelper
    {
        public static SortedList<T1, T2> CloneSortedList<T1, T2>(SortedList<T1, T2> list) where T2 : ICloneable
        {
            var clone = new SortedList<T1, T2>();
            foreach (var pair in list)
                clone.Add(pair.Key, (T2) pair.Value.Clone());
            return clone;
        }

        public static IEnumerable<T> CloneEnumerable<T>(IEnumerable<T> souce)
        {
            foreach (T item in souce)
            {
                var c = souce as ICloneable;
                yield return c != null ? (T) c.Clone() : item;
            }
                
        }

    }
}