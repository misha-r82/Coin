using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Btr.PrivApi;
using Lib.Annotations;

namespace Btr
{
    public class Seller
    {
        public Order BuyOrder { get;}
        public Order SellOrder { get; private set; }
        private TrackSettings _sett;
        private ApiParser _apiParser;
        public Seller(Order buyOrder, TrackSettings sett, ApiParser apiParser)
        {
            BuyOrder = buyOrder;
            _sett = sett;
            _apiParser = apiParser;
        }
        public async Task<bool> IsCpmplited()
        {
            return await _apiParser.IsComplited(SellOrder);
        }
        public async void TrySell(CoursePoint point, Gradient.Grad grad)
        {
            double minDelta = Math.Abs(grad.GPos / grad.GNeg) * _sett.Delta;
            if (minDelta < _sett.Delta) minDelta = _sett.Delta;
            if (point.Course < BuyOrder.Price *(1 + minDelta)) return;
            var sellOrder = new Order(BuyOrder.Pair, point.Course, BuyOrder.Amount);
            _apiParser.Sell(sellOrder);
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowSell))
            {
                var mrg = point.Course - BuyOrder.Price * (1 + _sett.Delta);
                Debug.WriteLine(string.Format("sell:{0} buy:{1} kd={2:0.000 00} mrg={3:0.000 00} mrg={4:0.000 00}",
                        point, BuyOrder.Point, (point.Course - BuyOrder.Price) /(_sett.Delta * BuyOrder.Price), mrg, mrg/ BuyOrder.Price * 100));
            }   
        }
    }
}