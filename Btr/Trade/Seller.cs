using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Btr.Log;
using Coin.Polon;
using Lib.Annotations;

namespace Coin
{
    [DataContract]
    public class Seller
    {
        [DataMember] public Order BuyOrder { get; private set; }
        [DataMember] public Order SellOrder { get; private set; }
        [DataMember] private double _delta;
        [DataMember] private IApiDriver _apiDriver;
        public Seller(Order buyOrder, double delta, IApiDriver apiDriver)
        {
            BuyOrder = buyOrder;
            _delta = delta;
            _apiDriver = apiDriver;
        }

        public Seller(Treader treder, double amount)
        {
            var price = treder.Tracker.Leap.LastPt.Course;
            BuyOrder = new Order(treder.Market.Name, price, amount);
            _delta = 0;
            _apiDriver = treder.Market.Api;
        }
        public async Task<bool> IsCpmplited()
        {
            return await _apiDriver.IsComplited(SellOrder);
        }
        public async Task TrySell(CoursePoint point)
        {
            if (point.Course - BuyOrder.Price < BuyOrder.Price * _delta) return;
            SellOrder = new Order(BuyOrder.Pair, point.Course, BuyOrder.Amount);
            await _apiDriver.Sell(SellOrder);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowSell))
            {
                var mrg = point.Course - BuyOrder.Price * (1 + _delta);
                Log.CreateLog("Sell", string.Format("sell:{0} buy:{1} kd={2:0.000 00} mrg={3:0.000 00} mrg={4:0.000 00}",
                        point, BuyOrder.Point, (point.Course - BuyOrder.Price) /(_delta * BuyOrder.Price), mrg, mrg/ BuyOrder.Price * 100));
            }   
        }
    }
}