using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Btr.Polon
{
    public class ApiBase
    {
        protected string ApiKey = "JEKJKAP9-R1TMMAPW-U5AYF3DD-X9FU4PMD";
        protected string ApiSecret = "03e596a58ac67bd06e7fc84d3da69c7665722fd7b94c509b390afb1e792e440c86f281f3e2294b281c120537fe294f567130ffef95fc4bb6a5c72f268cf8f7ed";
        protected string KeyPar = "Key";
        protected string SecretPar = "Sign";

        protected long Nonce {get { return DateTime.Now.Ticks; } }
        public async Task<string> MyFunc()
        {
            string url = "https://poloniex.com/tradingApi";
            var postPars = new Dictionary<string, string>();
            postPars.Add("command", "returnOpenOrders");
            postPars.Add("currencyPair", "all");
            postPars.Add("nonce", Nonce.ToString());
            string result = await SendPrivateApiRequestAsync(url, postPars);
            return result;
            //deserialize the result string to your liking
        }

        public async Task<string> Buy(Order order)
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
        public async Task<string> Sell(Order order)
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

        public async void CheckComplited(IEnumerable<Order> orders)
        {
            
        }


        private string GetParsStr(IDictionary<string, string> pars)
        {
            if (pars.Count == 0) return "";
            var sb = new StringBuilder();
            foreach (var par in pars)
                sb.AppendFormat("{0}={1}&", par.Key, par.Value);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        private async Task<string> SendPrivateApiRequestAsync(string privUrl, IDictionary<string, string> postPars)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(privUrl);
                var parsStr = GetParsStr(postPars);
                var strContent = new StringContent(parsStr);
                strContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Add(KeyPar, ApiKey);
                client.DefaultRequestHeaders.Add(SecretPar, genHMAC(parsStr));
                var result = await client.PostAsync(privUrl, strContent);
                return await result.Content.ReadAsStringAsync();
            }
        }
        private string genHMAC(string message)
        {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(ApiSecret));
            var messagebyte = Encoding.ASCII.GetBytes(message);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }
    }
}
