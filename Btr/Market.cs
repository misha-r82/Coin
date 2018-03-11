using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Coin.History;
using Lib;

namespace Coin
{
    [DataContract]
    public class Market
    {
        public Market(string name, IApiDriver api)
        {
            Name = name;
            Interval = TradeMan.Interval;
            CourseData = new CourseItem[0];
            _api = api;
        }
        [DataMember] public string Name { get; set; }
        [DataMember] private IApiDriver _api;
        [DataMember] private TimeSpan Interval { get; set; }
        private TimeSpan LoadDepth { get { return new TimeSpan(7,0,0,0);} }
        public CourseItem[] CourseData;
        public IApiDriver Api
        {
            get { return _api; }
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
        public IEnumerable<CourseItem> GetData(DatePeriod period)
        {
            int pos = Array.BinarySearch(CourseData, new CourseItem(period.From, 0, 0),
                new Course.DateComparer());
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
        /// <summary>
        /// Загружаем до текущей даты. последний элемент с неполными данными не добавляем
        /// true если добавлен элемент
        /// </summary>
        /// <returns></returns>
        public bool LoadHistory(DatePeriod period = null)
        {
            var course = new Course(_api);
            if (CourseData == null)// нет истории
            {
                if (CourseData.Length == 0)
                {
                    CourseData = new CourseItem[0];
                    var nowDate = DateTime.Now;
                    period = new DatePeriod(nowDate - LoadDepth, DateTime.Now);
                }
                else
                {
                    var last = CourseData[CourseData.Length -1].date;
                    period = new DatePeriod(last + Interval,DateTime.Now);                    
                }
            }
            var newData = course.GetHistory(Name, period, Interval).ToArray();
            int lastNotNul = -1;
            int pos = 0;
            foreach (CourseItem item in newData)
                if (item.course != 0) lastNotNul = pos++;
            if (lastNotNul < 1) return false;
            if (CourseData.Length == 0)
            {
                CourseData = newData;
                return true;
            }
            var joined = new CourseItem[CourseData.Length + lastNotNul + 1];
            Array.Copy(CourseData, joined, CourseData.Length);
            Array.Copy(newData, 0, joined, CourseData.Length, lastNotNul + 1);
            CourseData = joined;
            return true;
        }
    }
}
