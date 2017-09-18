using Btr.Data;
using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr.History
{
    public class PlnCouse
    {
        public TimeSpan LoadSize = new TimeSpan(1, 0, 0, 0);
        public struct CouseItem
        {
            public CouseItem(DateTime date, double course, double delta)
            {
                this.date = date;
                this.course = course;
                this.delta = delta;
            }
            public DateTime date;
            public double course;
            public double delta;
        }

        private void ShiftPeriod(DatePeriod period, TimeSpan delta)
        {
            period.From += delta;
            period.To += delta;
        }
        private IEnumerable<KVPair<DateTime, PlnHistoryItem[]>> GetData(
            string market, DatePeriod period, TimeSpan dlit)
        {
            var chunkEnd = period.From + LoadSize > period.To ?
                period.To : period.From + LoadSize;
            var loadPeriod = new DatePeriod(period.From, chunkEnd);
            var chunkPeriod = new DatePeriod(period.From, period.From + dlit);
            var chunk = new List<PlnHistoryItem>();
            do
            {
                PlnHistoryItem[] data = BtrHistory.GetHitoryPln(market, loadPeriod)
                    .OrderBy(d => d.date).ToArray();
                if (data.Length == 0) yield break;
                foreach (var item in data)
                {
                    while (chunk.Count == 0 && !chunkPeriod.IsConteins(item.date)) // не было торгов - - вернуть пустые
                    {
                        yield return new KVPair<DateTime, PlnHistoryItem[]>(chunkPeriod.From, new PlnHistoryItem[0]);
                        ShiftPeriod(chunkPeriod, dlit);
                    }                       
                    if (item.date > chunkPeriod.To)
                    {
                        yield return new KVPair<DateTime, PlnHistoryItem[]>(chunkPeriod.From, chunk.ToArray());
                        chunk.Clear();
                        ShiftPeriod(chunkPeriod, dlit);
                    }
                    chunk.Add(item);
                }
                ShiftPeriod(loadPeriod, LoadSize);
            } while (chunkPeriod.To < period.To);// неполные не возвращаем
        }
        public IEnumerable<CouseItem> GetCouse(string market, DatePeriod period, TimeSpan dlit)
        {
            foreach (var pair in GetData(market, period, dlit))
            {
                var chunk = pair.Val;
                var time = pair.Key;
                if (chunk.Length == 0) yield return new CouseItem();
                double sumAmount = 0;
                double sumValue = 0;
                for (int i = 0; i < chunk.Length; i++)
                {
                    var item = chunk[i];
                    sumValue += item.rate * item.amount;
                    sumAmount += item.amount;
                }
                double sred = sumValue / sumAmount;
                sumValue = 0;
                for (int i = 0; i < chunk.Length; i++)
                {
                    var item = chunk[i];
                    sumValue += (item.rate - sred) * item.amount;
                }
                double delta = sumValue / sumAmount;
                yield return new CouseItem(time, sred, delta);
            }
            /*
            DateTime first = from;
            do
            {
                DateTime end = to - first > MaxPeriod ? first + MaxPeriod : to;
                PlnHistoryItem[] data = BtrHistory.GetHitoryPln(market, first, end)
                    .OrderBy(d=>d.date).ToArray();
                if (data.Length == 0) yield break;        
                double sumAmount = 0;
                double sumCourse = 0;
                int pos = 0;
                first = data[0].date;
                foreach (var item in data)
                {
                    if (pos++ == data.Length -1) break;
                    if (item.date > first + dlit)
                    {
                        Debug.WriteLine(first);
                        if (sumAmount > 0) yield return new CouseItem(first, sumCourse / sumAmount);
                        sumCourse = 0;
                        sumAmount = 0;
                        first = first + dlit;
                    }
                    else
                    {
                        sumCourse += item.rate * item.amount;
                        sumAmount += item.amount;                        
                    }           

                }                
            } while (first + dlit < to);
            
        }*/
        }
    }
}
