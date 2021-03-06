﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Btr.Log;
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
            //From = DateTime.Now.Date - TradeMan.MinInterval;
            CourseData = new CourseItem[0];
            _api = api;
        }
        [DataMember] public string Name { get; set; }
        [DataMember] private IApiDriver _api;
        [DataMember] private TimeSpan Interval { get; set; }
        public DateTime From { get { return !CourseData.Any() ? new DateTime() : CourseData[0].date; } }
        public DateTime To { get { return !CourseData.Any() ? new DateTime() : CourseData[CourseData.Length -1].date; } } 
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
            int pos = Array.BinarySearch(CourseData, new CourseItem(period.From, 0, 0, 0),
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
            if (period == null)
            {
                var to = new DateTime( DateTime.Now.Ticks / Interval.Ticks * Interval.Ticks);
                if (CourseData.Length == 0) // нет истории
                    period = new DatePeriod(From, to);
                else
                {
                    period = new DatePeriod(To + Interval, to);
                }
            }
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowLoadingHistory))
                Log.CreateLog("LoadHistory", period.ToString());
            var newData = course.GetHistory(Name, period, Interval).ToArray();
            int lastNotNul = -1;
            for (int i = newData.Length - 1; i>-1; i--)
                if (newData[i].course != 0)
                {
                    lastNotNul = i;
                    break;
                }
            if (lastNotNul < 1)
                return false;
            var joined = new CourseItem[CourseData.Length + lastNotNul];
            Array.Copy(CourseData, joined, CourseData.Length);
            Array.Copy(newData, 0, joined, CourseData.Length, lastNotNul);
            CourseData = joined;
            return true;
        }

    }
}
