using System;
using System.Collections.Generic;
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
        protected string ApiKey = "PWCPRD75-7Q3MGCZM-RCGOGM6O-KIY3DNNT";
        protected string ApiSecret = "61f7c40b95cbb73062c00352d40411ee4a44ad23ef3719dcd8ceab7c8c29447bd1ca4c8e66819ac063af7dbffc6466c1cb0a3e7bbfb22bfb765595e89d89a6f8";
        protected string KeyPar = "Key";
        protected string SecretPar = "Sign";
        public async Task<string> MyFunc()
        {
            long nonce = DateTime.Now.Ticks;
            string url = "https://poloniex.com/tradingApi";
            string myParam = "command=returnOpenOrders&currencyPair=all&nonce=" + nonce;
            var postPars = new Dictionary<string, string>();
            postPars.Add("currencyPair", "all");
            string result = await SendPrivateApiRequestAsync(url, myParam, postPars);
            return result;
            //deserialize the result string to your liking
        }
        private async Task<string> SendPrivateApiRequestAsync(string privUrl, string myParam, IDictionary<string, string> postPars)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(privUrl);
                var myContent = new StringContent(myParam);
                var encodedContent = new FormUrlEncodedContent(postPars);

                myContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Add(KeyPar, ApiKey);
                client.DefaultRequestHeaders.Add(SecretPar, genHMAC(myParam));
                
                var result = await client.PostAsync(privUrl, myContent);
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
