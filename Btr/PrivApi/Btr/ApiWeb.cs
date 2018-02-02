using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace Coin.Btr
{
    public class ApiWeb : ApiWebBase
    {
        public override async Task<string> Buy(Order order)
        {
            string url = "https://poloniex.com/tradingApi";
            var postPars = new Dictionary<string, string>();
            postPars.Add("command", "buy");
            postPars.Add("currencyPair", order.Pair);
            postPars.Add("rate", order.Price.ToString(CultureInfo.InvariantCulture));
            postPars.Add("amount", order.Amount.ToString(CultureInfo.InvariantCulture));
            postPars.Add("nonce", Nonce.ToString());
            string result = await SendPrivateApiRequestAsync(url, postPars);
            return result;
        }

        public override async Task<string> Sell(Order order)
        {
            string url = "https://poloniex.com/tradingApi";
            var postPars = new Dictionary<string, string>();
            postPars.Add("command", "sell");
            postPars.Add("currencyPair", order.Pair);
            postPars.Add("rate", order.Price.ToString(CultureInfo.InvariantCulture));
            postPars.Add("amount", order.Amount.ToString(CultureInfo.InvariantCulture));
            postPars.Add("nonce", Nonce.ToString());
            string result = await SendPrivateApiRequestAsync(url, postPars);
            return result;
        }

        public override async Task<string> CanselOrder(Order order)
        {
            string url = "https://poloniex.com/tradingApi";
            var postPars = new Dictionary<string, string>();
            postPars.Add("command", "cancelOrder");
            postPars.Add("orderNumber", order.Id.ToString());
            postPars.Add("nonce", Nonce.ToString());
            string result = await SendPrivateApiRequestAsync(url, postPars);
            return result;
        }

        public override async Task<string> TradeHistory(string pair, DatePeriod period)
        {
            ulong fromStamp = Utils.DateTimeToUnixTimeStamp(period.From);
            ulong toStamp = Utils.DateTimeToUnixTimeStamp(period.To);
            string url = "https://poloniex.com/tradingApi";
            var postPars = new Dictionary<string, string>();
            postPars.Add("command", "returnTradeHistory");
            postPars.Add("currencyPair", pair);
            postPars.Add("start", fromStamp.ToString());
            postPars.Add("end", toStamp.ToString());
            postPars.Add("nonce", Nonce.ToString());
            string result = await SendPrivateApiRequestAsync(url, postPars);
            return result;
        }
    }
}
