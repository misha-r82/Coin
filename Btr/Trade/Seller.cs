using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Coin.Polon;
using Lib.Annotations;

namespace Coin
{
    [DataContract]
    public class Seller
    {
        [DataMember] public Order BuyOrder { get;}
        [DataMember] public Order SellOrder { get; private set; }
        [DataMember] private TrackSettings _sett;
        [DataMember] private IApiDriver _apiDriver;
        public Seller(Order buyOrder, TrackSettings sett, IApiDriver apiDriver)
        {
            BuyOrder = buyOrder;
            _sett = sett;
            _apiDriver = apiDriver;
        }
        public async Task<bool> IsCpmplited()
        {
            return await _apiDriver.IsComplited(SellOrder);
        }
        public async Task TrySell(CoursePoint point, Gradient.Grad grad)
        {
            if (SellOrder != null) return;
            SellOrder = new Order(BuyOrder.Pair, point.Course, BuyOrder.Amount);
            await _apiDriver.Sell(SellOrder);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowSell))
            {
                var mrg = point.Course - BuyOrder.Price * (1 + _sett.Delta);
                Debug.WriteLine(string.Format("sell:{0} buy:{1} kd={2:0.000 00} mrg={3:0.000 00} mrg={4:0.000 00}",
                        point, BuyOrder.Point, (point.Course - BuyOrder.Price) /(_sett.Delta * BuyOrder.Price), mrg, mrg/ BuyOrder.Price * 100));
            }   
        }
    }
}