using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using Lib;

namespace Lib
{
    public class DataCache<Tkey, Tval>
    {
        public int max_count;
        public DataCache(int max_count = 16000) 
        {
            dataList = new SortedList<Tkey, List<KVPair<DatePeriod, Tval[]>>>();
            this.max_count = max_count;
        }
        private SortedList<Tkey, List<KVPair<DatePeriod, Tval[]>>> dataList;
        public bool CanReturnFromCashe(Tkey par, DatePeriod period)
        {
            if (!dataList.ContainsKey(par)) return false;
            if (period == null) return true; // нет нужды в периодах idInfo
            return dataList[par].Any(kvPair => kvPair.Key != null && kvPair.Key.IsConteins(period));
        }

        public void Add(Tkey key, IEnumerable<Tval> val, DatePeriod period)
        {
            lock (dataList)
            {
                if (dataList.Count > max_count)
                {
                    int mid = dataList.Count - max_count;
                    for (int i = 0; i < mid; i++) // удаляем половину
                        dataList.RemoveAt(0);
                }
                if (!dataList.ContainsKey(key))
                    dataList.Add(key, new List<KVPair<DatePeriod, Tval[]>>());
                if (period != null)
                {
                    dataList[key] = dataList[key].Where( p => !period.IsIntesect(p.Key))// удаление всех пересечений
                        .OrderBy(p=>(p.Key == null) ? new DateTime(0) : p.Key.From).ToList();  
                    dataList[key].Add(new KVPair<DatePeriod, Tval[]>(period, val.ToArray()));              
                }
                else
                {
                    var nullPair = dataList[key].FirstOrDefault(x => x.Key == null);
                    if (nullPair == null)
                        dataList[key].Add(new KVPair<DatePeriod, Tval[]>(null, val.ToArray()));
                    else nullPair.Val = val.ToArray();
                }                
            }


            
        }
// пока не используется
        //private DatePeriod[] getPeriods(Tkey key, DatePeriod period)
        //{
        //    var rezList = new List<DatePeriod>();
        //    foreach (KVPair<DatePeriod, Tval[]> kvPair in dataList[key])
        //    {
        //        if (kvPair.Key.IsIntesect(period))
        //            rezList.Add(kvPair.Key);
        //    }
        //    return rezList.OrderBy(p => p.From).ToArray();
        //}
        //public DatePeriod GetNececeryPeriod(Tkey key, DatePeriod sourcePeriod) 
        //{
        //    var periods = getPeriods(key, sourcePeriod);
        //    switch (periods.Length)
        //    {
        //        case 0: return sourcePeriod;
        //        case 1: return sourcePeriod - periods[0];
        //        case 2: return sourcePeriod - periods[0] - periods[1];
        //        default: return sourcePeriod;
        //    }
        //}
        public IEnumerable<Tval> GetData(Tkey key, DatePeriod period = null)
        {
            if (period == null) // возвращаем все что есть по ключу null
            {
                foreach (KVPair<DatePeriod, Tval[]> kVPair in dataList[key])
                    if (kVPair.Key == null)
                            foreach (var element in kVPair.Val)
                                yield return element;     
                yield break;
            }
            foreach (KVPair<DatePeriod, Tval[]> kvPair in dataList[key])
                if (kvPair.Key != null && kvPair.Key.IsConteins(period))
                    foreach (Tval val in kvPair.Val)
                        yield return val;
        }
    }
}
