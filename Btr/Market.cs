using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Btr.History;
using Lib;

namespace Btr
{
    [DataContract]
    public class Market
    {
        public TimeSpan Tmin = new TimeSpan(0,0,5);
        public Market(string name)
        {
            Name = name;
            var from = (DateTime.Now - MultiPeriodGrad.MaxPeriod).Date;
            LoadHistory(new DatePeriod(from, GetTo(from, DateTime.Now)));
        }

        protected static DateTime GetTo(DateTime from, DateTime to)
        {
            var delta = to - from;
            delta = new TimeSpan(TradeMan.Interval.Ticks * (delta.Ticks / TradeMan.Interval.Ticks));
            return from + delta;
        }
        public CoursePoint LastPt
        {
            get
            {
                if (CourseData == null || CourseData.Length == 0) return new CoursePoint();
                var course = CourseData[CourseData.Length - 1];
                return new CoursePoint(course.course, course.date);
            }
        }

        [DataMember] public string Name { get; set; }
        public PlnCouse.CouseItem[] CourseData;
        public IEnumerable<PlnCouse.CouseItem> GetData(DatePeriod period)
        {
            int pos = Array.BinarySearch(CourseData, new PlnCouse.CouseItem(period.From, 0, 0),
                new PlnCouse.DateComparer());
            if (pos < 0) pos = ~pos;
            if (pos < 0) yield break; 
            while (pos < CourseData.Length && period.IsConteins(CourseData[pos].date))
                yield return CourseData[pos++];
        }
        public double Getourse(DateTime date)
        {
            if (CourseData.Length < 2) throw new Exception("Данные не загружены");
            int lastIndex = CourseData.Length - 1;
            if (CourseData[CourseData.Length - 1].date < date || date < CourseData[0].date)
                throw new ArgumentException(string.Format("Дата {0} за пределами диапаазона {1} - {2}",
                    date, CourseData[0].date, CourseData[lastIndex].date));
            for (int i = CourseData.Length -1; i > 1; i--)
            {
                if (CourseData[i].date == date) return CourseData[i].course;
                if (date < CourseData[i].date && date > CourseData[i - 1].date)
                    return CourseData[i - 1].course;
            }
            return -1;
        }
        public void ReloadNew()
        {
            var course = new PlnCouse();
            var last = CourseData[CourseData.Length -1].date;
            DateTime from = last.AddSeconds(1);
            var period = new DatePeriod(from, GetTo(from, DateTime.Now));
            var newData = course.GetHistory(Name, period, TradeMan.Interval).ToArray();
            var joined = new PlnCouse.CouseItem[newData.Length + CourseData.Length];
            Array.Copy(CourseData, joined, CourseData.Length);
            Array.Copy(newData, 0, joined, CourseData.Length, newData.Length);
        }
        public void LoadHistory(DatePeriod period)
        {
            var course = new PlnCouse();
            CourseData = course.GetHistory(Name, period, TradeMan.Interval).ToArray();
        }
    }
}
