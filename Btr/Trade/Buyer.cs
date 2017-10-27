using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Btr.PrivApi;
using Lib;

namespace Btr
{
    [DataContract]
    public class Buyer
    {
        public Buyer(ApiParser apiParser)
        {
            _apiParser = apiParser;
        }

        [DataMember] public double Balance { get; set; }
        [DataMember] public double PartsInvest { get; set; }
        [DataMember] private ApiParser _apiParser;
        [DataMember] private Order _Order { get; set; }


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
            if (_Order == null) return false;
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