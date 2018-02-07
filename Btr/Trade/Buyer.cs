using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Coin.Polon;
using Lib;

namespace Coin
{
    [DataContract]
    public class Buyer
    {
        public Buyer(IApiDriver apiDriver)
        {
            _apiDriver = apiDriver;
        }

        [DataMember] public double Balance { get; set; }
        [DataMember] public double PartsInvest { get; set; }
        [DataMember] private IApiDriver _apiDriver;
        [DataMember] private Order _Order { get; set; }


        public void Buy(CoursePoint buyPoint, CourseTracker tracker)
        {
            Gradient.Grad grad = tracker.GPrew;
            LeapInfo leap = tracker.Leap;
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * tracker.Sett.Delta;

            if (minDelta < tracker.Sett.Delta) minDelta = tracker.Sett.Delta;
            if (buyPoint.Course > leap.DownBegin.Course * (1 - minDelta)) return;
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowBuy))
                Debug.WriteLine("Buy={0} {1}", buyPoint, tracker.Leap.Mode);
            _Order = new Order(tracker.Market.Name,  buyPoint.Course, Balance / PartsInvest);
            _Order.PlaceDate = buyPoint.Date; // для отладки по истории, в реальном случае поле затрется
            _apiDriver.Buy(_Order);
        }

        public async Task<bool> IsCpmplited()
        {
            if (_Order == null) return false;
            return await _apiDriver.IsComplited(_Order);
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