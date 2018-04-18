using Lib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coin.Data;

namespace Coin.History
{
    public partial class Course
    {
        
        public TimeSpan LoadSize = new TimeSpan(0, 1, 0, 0);
        public Course(IApiDriver api)
        {
            _api = api;
        }
        private IApiDriver _api;
        private void ShiftPeriod(DatePeriod period, TimeSpan delta)
        {
            period.From += delta;
            period.To += delta;
        }

        // за время принято From
        private IEnumerable<KVPair<DateTime, HistoryItem[]>> GetData(
            string market, DatePeriod period, TimeSpan interval)
        {
            
            var chunkEnd = period.From + LoadSize > period.To ?
                period.To : period.From + LoadSize;
            var loadPeriod = new DatePeriod(period.From, chunkEnd);

            var chunkPeriod = new DatePeriod(period.From, period.From + interval);
            var chunk = new List<HistoryItem>();
            do
            {
                HistoryItem[] data = _api.GetHitory(market, loadPeriod)
                    .OrderBy(d => d.date).ToArray();
                Thread.Sleep(200);
                Debug.WriteLine("*{0}",loadPeriod);
                if (data.Length == 0) yield break;
                int pos = 0;
                do
                {
                    var item = data[pos];
                    while (chunkPeriod.IsConteins(item.date) && pos < data.Length - 1)
                    {
                        chunk.Add(item);
                        item = data[++pos];
                    }
                    while (!chunkPeriod.IsConteins(item.date) || !(pos < data.Length - 1))
                    {
                        //Debug.WriteLine("{0}", chunkPeriod);
                        yield return new KVPair<DateTime, HistoryItem[]>(chunkPeriod.From, chunk.ToArray());
                        chunk.Clear();
                        ShiftPeriod(chunkPeriod, interval);     
                        if (chunkPeriod.To > loadPeriod.To) break;                  
                    }

                } while (chunkPeriod.To <= loadPeriod.To);
                ShiftPeriod(loadPeriod, LoadSize);

                if (loadPeriod.To + interval > period.To && loadPeriod.From < period.To) // последний
                    loadPeriod.To = period.To;
            } while (chunkPeriod.To <= period.To);
        }
        public IEnumerable<CourseItem> GetHistory(string market, DatePeriod period, TimeSpan interval)
        {
            var data = GetData(market, period, interval).ToArray();
            foreach (var pair in data)
            {
                var chunk = pair.Val;
                var time = pair.Key;
                if (chunk.Length == 0)
                {
                    yield return new CourseItem(time, 0, 0);
                    continue;
                }
                double delta = 0;
                double sred = chunk[0].rate;
                if (chunk.Length > 1)
                {
                    double sumAmount = 0;
                    double sumValue = 0;
                    int half = chunk.Length / 2;
                    for (int i = 0; i < half; i++)
                    {
                        var item = chunk[i];
                        sumValue += item.rate * item.amount;
                        sumAmount += item.amount;
                    }
                    double sred1 = sumValue / sumAmount;
                    for (int i = half; i < chunk.Length; i++)
                    {
                        var item = chunk[i];
                        sumValue += item.rate * item.amount;
                        sumAmount += item.amount;
                    }
                    double sred2 = sumValue / sumAmount;
                    sred = (sred1 + sred2) / 2;
                    delta = sred2 - sred1;
                }
                yield return new CourseItem(time, sred, delta * 2);   
            }


        }
        
        public class DateComparer : IComparer<CourseItem>
        {
            public int Compare(CourseItem x, CourseItem y)
            {
                return x.date.CompareTo(y.date);
            }
        }
    }
}
