using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Btr.Polon;
using Lib;
using Newtonsoft.Json;

namespace Btr.PrivApi
{
    [DataContract]
    public class ApiParser
    {
        private readonly TimeSpan TIME_GAP = new TimeSpan(0,0,1);
        public ApiParser(ApiBase api, bool testMode = false)
        {
            Api = api;
            _testMode = testMode;
        }
        private ApiBase Api { get; }
        private bool _testMode;
        private int _testId = 0;
        public async Task Buy(Order order)
        {
            if (_testMode)
            {
                order.Id = _testId++;
                return;
            }
            string res = await Api.Buy(order);
            var resp = JsonConvert.DeserializeObject<BuyResponce>(res);
            resp.SetOrder(order);
        }
        public async Task Sell(Order order)
        {
            if (_testMode)
            {
                order.ComplitedDate = order.PlaceDate;
                order.Id = _testId++;
                return;
            }
            string res = await Api.Sell(order);
            var resp = JsonConvert.DeserializeObject<BuyResponce>(res);
            resp.SetOrder(order);
        }
        public async Task CanselOrder(Order order)
        {
            string res = await Api.CanselOrder(order);
            var resp = JsonConvert.DeserializeObject<CanselResponse>(res);
            if (resp.success != 1) throw new InvalidOperationException( string.Format(
            "Can't cansel order №{0}", order.Id));
        }
        public async Task<Order[]> OrderHistory(string pair, DatePeriod period)
        {
            string res = await Api.TradeHistory(pair, period);
            var resp = JsonConvert.DeserializeObject<OrderPln[]>(res);
            return resp.Select(o => o.Order).ToArray();

        }
        public async Task<bool> IsComplited(Order order)
        {
            if (order == null || order.Id < 1) return false;
            if (_testMode)
            {
                order.ComplitedDate = order.PlaceDate;
                return true;
            }
            Order[] res = await OrderHistory(order.Pair, 
                new DatePeriod(order.PlaceDate - TIME_GAP, DateTime.Now));
            var complOrder = res.FirstOrDefault(o => o.Id == order.Id);
            if (complOrder.ComplitedDate == new DateTime(0)) return false;
            order.ComplitedDate = complOrder.ComplitedDate;
            order.Amount = complOrder.Amount;
            order.Price = complOrder.Price;
            return true;
        }
        private class BuyResponce
        {
            public long orderNumber;
            public Trade[] resultingTrades;
            public DateTime Date { get { return resultingTrades.Max(t => t.date); } }
            public double Amount { get { return resultingTrades.Sum(t => t.amount); } }

            public void SetOrder(Order order)
            {
                order.Id = orderNumber;
                order.PlaceDate = Date;
                order.Price = Amount / order.Amount;
                order.Amount = Amount;
            }
        }
        private class Trade
        {
            public DateTime date;
            public double amount;
            public double price;
        }
        private class CanselResponse
        {
            public long success;
        }
        private class OrderPln
        {
            public Order Order
            {
                get
                {
                    var order = new Order("", rate, amount);
                    order.ComplitedDate = date;
                    order.Id = orderNumber;
                    return order;
                }
            }
            public long orderNumber;
            public DateTime date;
            public double amount;
            public double rate;
        }
    }
}
