using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Coin.Data;
using Coin.Polon;
using Lib;
using Newtonsoft.Json;

namespace Coin.Polon
{
    [DataContract]
    public class ApiDriver : IApiDriver
    {
        const string URI_PLN_PATT = "https://poloniex.com/public?command=returnTradeHistory&currencyPair={0}&start={1}&end={2}";
        private readonly TimeSpan TIME_GAP = new TimeSpan(0,0,1);
        public ApiDriver()
        {
            Api = new ApiWeb();        
        }
        public string Name
        {
            get { return "PLN"; }
        }
        public ApiWebBase Api { get; }
        

        private int _testId = 0;
        public async Task Buy(Order order)
        {
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ApiEmulate))
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
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ApiEmulate))
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
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ApiEmulate))
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
        public HistoryItem[] GetHitory(string market, DatePeriod period)
        {
            ulong fromStamp = Utils.DateTimeToUnixTimeStamp(period.From);
            ulong toStamp = Utils.DateTimeToUnixTimeStamp(period.To);
            var uri = string.Format(URI_PLN_PATT, market, fromStamp, toStamp);
            HistoryItem[] result = new HistoryItem[0];
            int max_attempt = 20;
            int attempts = 0;
            var apiCall = new Api.ApiCall(false);
            bool isErr = false;
            do
            {
                try
                {
                    result = apiCall.CallWithJsonResponse<HistoryItem[]>(uri);
                    isErr = false;
                }
                catch (Exception e)
                {
                    isErr = true;
                    Thread.Sleep(500);
                    if (attempts++ > max_attempt) throw new Exception("не удалось получить данные курса", e);                    
                }
            } while (isErr);
            return result.Where(i => period.IsConteins(i.date)).ToArray(); // из за погрешностей преобразования времени могут быть выходящие за исходный период
        }


        private class BuyResponce
        {
            public long orderNumber;
            public Trade[] resultingTrades;

            public DateTime Date
            {
                get
                {
                    if (resultingTrades == null) return new DateTime();
                    return resultingTrades.Max(t => t.date);
                }
            }

            public double Amount
            {
                get
                {
                    if (resultingTrades == null) return 0;
                    return resultingTrades.Sum(t => t.amount);
                }
            }

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
