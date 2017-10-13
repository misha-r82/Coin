using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Btr.Polon;
using Lib;
using Newtonsoft.Json;

namespace Btr.PrivApi
{
    public class ApiParser
    {
        public ApiParser(ApiBase api)
        {
            Api = api;
        }
        private ApiBase Api { get; }

        public async Task Buy(Order order)
        {
            string res = await Api.Buy(order);
            var resp = JsonConvert.DeserializeObject<BuyResponce>(res);
            order.Id = resp.orderNumber;
        }
        public async Task Sell(Order order)
        {
            string res = await Api.Sell(order);
            var resp = JsonConvert.DeserializeObject<BuyResponce>(res);
            order.Id = resp.orderNumber;
        }

        public async Task<Order[]> OrderHistory(string pair, DatePeriod period)
        {
            string res = await Api.TradeHistory(pair, period);
            var resp = JsonConvert.DeserializeObject<OrderPln[]>(res);
            return resp.Select(o => o.Order).ToArray();

        }
        private class BuyResponce
        {
            public long orderNumber;
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
