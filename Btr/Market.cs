﻿using System;
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
        public Market(string name)
        {
            Name = name;
            Interval = TradeMan.Interval;
        }
        private TimeSpan Interval { get; }

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
        public CourseItem[] CourseData;
        public IEnumerable<CourseItem> GetData(DatePeriod period)
        {
            int pos = Array.BinarySearch(CourseData, new CourseItem(period.From, 0, 0),
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
        protected DateTime GetTo(DateTime from, DateTime to)
        {
            var delta = to - from;
            delta = new TimeSpan(Interval.Ticks * (delta.Ticks / Interval.Ticks));
            return from + delta;
        }
        public void ReloadNew()
        {
            if (CourseData == null || CourseData.Length == 0)
                throw new Exception("Course Data is not initialized!");

            var course = new PlnCouse();
            var last = CourseData[CourseData.Length -1].date;
            var period = new DatePeriod(last, GetTo(last, DateTime.Now));
            var newData = course.GetHistory(Name, period, Interval).ToArray();
            var joined = new CourseItem[newData.Length + CourseData.Length];
            Array.Copy(CourseData, joined, CourseData.Length);
            Array.Copy(newData, 0, joined, CourseData.Length, newData.Length);
            CourseData = joined;
        }
        public void LoadHistory(DatePeriod period)
        {
            var from = period.From.Date;
            var newPeriod = new DatePeriod(from, GetTo(from, period.To));
            var course = new PlnCouse();
            CourseData = course.GetHistory(Name, newPeriod, Interval).ToArray();
        }
    }
}
