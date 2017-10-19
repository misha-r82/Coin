using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Btr.PrivApi;
using Lib;

namespace Btr
{
    public class Buyer
    {
        public Buyer(ApiParser apiParser)
        {
            throw new NotImplementedException();
        }

        public double Balance { get; set; }
        public double PartsInvest { get; set; }

        private ApiParser _apiParser;
        private Order _Order { get; set; }
        private DateTime _buyDate { get; set; }

        public void Buy(CoursePoint buyPoint, CourseTracker tracker)
        {
            Gradient.Grad grad = tracker.MulGradient;
            LeapInfo leap = tracker.Leap;
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * tracker.Sett.Delta;

            if (minDelta < tracker.Sett.Delta) minDelta = tracker.Sett.Delta;
            if (buyPoint.Course > leap.DownBegin.Course * (1 - minDelta)) return;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowBuy))
                Debug.WriteLine(string.Format("Buy={0} {1} g+/g-={2:0.00000}", buyPoint, tracker.Leap.Mode, grad.GPos / grad.GNeg));
            var order = new Order(tracker.Market.Name,  buyPoint.Course, Balance / PartsInvest);
            _apiParser.Buy(order);            
        }

        public Order GetComplited()
        {
            if (_Order == null || _Order.Id < 1) return null;
            var task = _apiParser.OrderHistory(_Order.Pair, new DatePeriod(_buyDate.AddMinutes(-3), DateTime.Now));
            task.Wait();
            if (!task.Result.Any(o => o.Id == _Order.Id)) return null;
            var res = _Order;
            _Order = null;
            return res;
        }
    }
}