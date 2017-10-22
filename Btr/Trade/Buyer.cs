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
            _apiParser = apiParser;
        }

        public double Balance { get; set; }
        public double PartsInvest { get; set; }

        private ApiParser _apiParser;
        private Order _Order { get; set; }


        public void Buy(CoursePoint buyPoint, CourseTracker tracker)
        {
            Gradient.Grad grad = tracker.MulGradient;
            LeapInfo leap = tracker.Leap;
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * tracker.Sett.Delta;

            if (minDelta < tracker.Sett.Delta) minDelta = tracker.Sett.Delta;
            if (buyPoint.Course > leap.DownBegin.Course * (1 - minDelta)) return;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowBuy))
                Debug.WriteLine("Buy={0} {1} g+/g-={2:0.00000}", buyPoint, tracker.Leap.Mode, grad.GPos / grad.GNeg);
            var order = new Order(tracker.Market.Name,  buyPoint.Course, Balance / PartsInvest);
            _apiParser.Buy(order);
        }

        public async Task<bool> IsCpmplited()
        {
            return await _apiParser.IsComplited(_Order);
        }
        public Order PopComplited()
        {
            if (!_Order.IsComplited) return null;
            var res = _Order;
            _Order = null;
            return res;             
        }
    }
}