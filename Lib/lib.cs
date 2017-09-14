using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using YAK_DB_Config;

namespace Lib
{
 
    [Serializable]
    [DataContract]
    public class DatePeriod : INotifyPropertyChanged, ICloneable
    {
        public class IntersectComparer : IComparer<DatePeriod>
        {
            public int Compare(DatePeriod d1, DatePeriod d2)
            {
                if (d1.From > d2.To) return 1;
                if (d2.From > d1.To) return -1;
                return 0;
            }
        }
        [DataMember]private DateTime from;
        [DataMember] private DateTime to;
        public DatePeriod()
        {
            from = DateTime.Now;
            to = DateTime.Now;
        }

        public DatePeriod(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }


        public DateTime From
        {
            get { return @from; }
            set
            {
                @from = value;
                OnPropertyChanged(nameof(From));
                OnPropertyChanged(nameof(Dlit));
            }
        }

        public DateTime To
        {
            get { return to; }
            set
            {
                to = value;
                OnPropertyChanged(nameof(To));
                OnPropertyChanged(nameof(Dlit));
            }
        }

        public static DatePeriod operator -(DatePeriod p1, DatePeriod p2)
        {
            if (!p1.IsIntesect(p2)) return null;
            if (p1.IsConteins(p2) || p2.IsConteins(p1)) return null;
            if (p1.from < p2.From) return new DatePeriod(p1.from, p2.from);
            return new DatePeriod(p1.from, p2.to);
        }
        public bool IsConteins(DatePeriod period2)
        {
            return (from <= period2.from && to >= period2.to);
        }
        public bool IsConteins(DateTime date)
        {
            return (from <= date && to >= date);
        }

        public bool IsIntesect(DatePeriod p2)
        {
            if (p2 == null) return false;
            return IsConteins(p2.From) || IsConteins(p2.To) || p2.IsConteins(this); // содержит другой либо сам содержится в нем
        }
        public DatePeriod Intersect(DatePeriod p2)
        {
            if (!IsIntesect(p2)) return null;
            return new DatePeriod( IsConteins(p2.From) ? p2.From : From, IsConteins(p2.To) ? p2.To : To );
        }
        public IEnumerable<DatePeriod> Split(TimeSpan part)
        {
            DateTime curStart = From;
            while (curStart < to)
            {
                DateTime curEnd = curStart + part < to ? curStart + part : to;
                yield return new DatePeriod(curStart, curEnd);
                curStart = curEnd;
            }
        }

        public TimeSpan Dlit { get { return to - from; } }
        public object Clone()
        {
            return MemberwiseClone();
        }

        public static IEnumerable<DatePeriod> ToParts(DatePeriod period, TimeSpan interval)
        {
            DatePeriod next = new DatePeriod(period.from, period.to);
            do
            {
                next.to = next.from + interval;
                if (next.to.Ticks < period.to.Ticks)
                {
                    yield return new DatePeriod(next.from, next.to);
                    next.from = next.to;
                }
                else
                {
                    next = new DatePeriod(next.from, period.to);
                    yield return next;
                    break;
                }
            } while (true);
        }
        public override string ToString()
        {
            return String.Format("from: {0} to {1}", from, to);
        }
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public static class SInicator
    {
        private static FrmStatus status;
        private static FrmSatatus2x status2x;
        public static Window OwnerWindow;

        public static void ShowIndicator2x(StatusIndicator si1, StatusIndicator si2)
        {

            status2x = new FrmSatatus2x();
            if (OwnerWindow != null) status2x.Owner = OwnerWindow;
            status2x.Status1 = si1;
            status2x.Status2 = si2;
            status2x.Show();
        }
        public static void ShowIndicator(StatusIndicator si)
        {           
            status = new FrmStatus(si);
            if (OwnerWindow != null) status.Owner = OwnerWindow;
            status.Show();
        }

        public static void HideStatus()
        {
            if (status != null) status.Close();
        }

        public static void HideStatus2x()
        {
            if (status2x != null) status2x.Close();
            //status2x.Dispatcher.BeginInvoke(DispatcherPriority.Background, new ThreadStart(() => status2x.Close()));
        }
    }
}
