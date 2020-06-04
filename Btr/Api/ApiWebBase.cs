using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Lib;

namespace Coin
{
    public abstract class ApiWebBase
    {
        public string ApiKey { get; set; } = "EKAYYJYA-4FTPMKE9-ZVWKRJWY-BX0ZB7NG";
        public string ApiSecret { get; set; } = "07746e0623fa03a6b5ee81b773c3b3bd8c8285e4c36f5dcb6a08b8b2abae810c292e6fdef29f109989dc943e3e58e4adcd97dd25d2941b2ae2e86df34efe9892";

        protected string KeyPar = "Key";
        protected string SecretPar = "Sign";
        protected long Nonce {get { return DateTime.Now.Ticks; } }

        public abstract Task<string> Buy(Order order);
        public abstract Task<string> Sell(Order order);
        public abstract Task<string> CanselOrder(Order order);
        public abstract  Task<string> TradeHistory(string pair, DatePeriod period);

        protected string GetParsStr(IDictionary<string, string> pars)
        {
            if (pars.Count == 0) return "";
            var sb = new StringBuilder();
            foreach (var par in pars)
                sb.AppendFormat("{0}={1}&", par.Key, par.Value);
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }
        protected async Task<string> SendPrivateApiRequestAsync(string privUrl, IDictionary<string, string> postPars)
        {
            using (var client = new HttpClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.BaseAddress = new Uri(privUrl);
                var parsStr = GetParsStr(postPars);
                var strContent = new StringContent(parsStr);
                strContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Add(KeyPar, ApiKey);
                client.DefaultRequestHeaders.Add(SecretPar, (string) genHMAC(parsStr));
                var result = await client.PostAsync(privUrl, strContent);
                return await result.Content.ReadAsStringAsync();
            }
        }
        protected string genHMAC(string message)
        {
            var hmac = new HMACSHA512(Encoding.ASCII.GetBytes(ApiSecret));
            var messagebyte = Encoding.ASCII.GetBytes(message);
            var hashmessage = hmac.ComputeHash(messagebyte);
            var sign = BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
            return sign;
        }
    }
}