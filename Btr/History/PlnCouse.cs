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
        public TimeSpan LoadSize = new TimeSpan(0, 4, 0, 0);
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
                    if (chunkPeriod.IsConteins(item.date))
                        chunk.Add(item);
                    else
                    {
                        yield return new KVPair<DateTime, PlnHistoryItem[]>(chunkPeriod.From, chunk.ToArray());
                        chunk.Clear();
                        ShiftPeriod(chunkPeriod, dlit);
                    }

                }
                ShiftPeriod(loadPeriod, LoadSize);
            } while (chunkPeriod.To < period.To);// неполные не возвращаем
        }
        public IEnumerable<CouseItem> GetHistory(string market, DatePeriod period, TimeSpan dlit)
        {
            foreach (var pair in GetData(market, period, dlit))
            {
                var chunk = pair.Val;
                var time = pair.Key;
                if (chunk.Length == 0)
                {
                    yield return new CouseItem(time, 0, 0);
                    continue;
                }
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


        }
        public class DateComparer : IComparer<CouseItem>
        {
            public int Compare(CouseItem x, CouseItem y)
            {
                return x.date.CompareTo(y.date);
            }
        }
    }
}
