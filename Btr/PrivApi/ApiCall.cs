﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Coin;
using Coin.Api;

namespace Bittrex
{
    public class ApiCall
    {
        private readonly bool simulate;

        public ApiCall(bool simulate)
        {
            this.simulate = simulate;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            // Skip validation of SSL/TLS certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public T CallWithJsonResponse<T>(string uri, bool hasEffects, params Tuple<string, string>[] headers)
        {
            if (simulate && hasEffects)
            {
                Debug.WriteLine("(simulated)" + GetCallDetails(uri));
                return default(T);
            }
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowUri))
                Debug.WriteLine(GetCallDetails(uri));
            var request = HttpWebRequest.CreateHttp(uri);
            foreach (var header in headers)
            {
                request.Headers.Add(header.Item1, header.Item2);
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var content = sr.ReadToEnd();
                        var js = JsonConvert.DeserializeObject<T>(content);
                        ApiCallResponse<T> jsonResponse = JsonConvert.DeserializeObject<ApiCallResponse<T>>(content);

                        if (jsonResponse.success)
                        {
                            return jsonResponse.result;
                        }
                        else
                        {
                            throw new Exception(jsonResponse.message.ToString() + "Call Details=" + GetCallDetails(uri));
                        }
                    }
                }
                else
                {
                    throw new Exception("Error - StatusCode=" + response.StatusCode + " Call Details=" + GetCallDetails(uri));
                }
            }
        }
        public T CallWithJsonResponse<T>(string uri)
        {
            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowUri))
                Debug.WriteLine(GetCallDetails(uri));
            var request = HttpWebRequest.CreateHttp(uri);            
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var content = sr.ReadToEnd();
                        T jsonResponse = JsonConvert.DeserializeObject<T>(content);
                        return jsonResponse;                        
                    }
                }
                else
                {
                    throw new Exception("Error - StatusCode=" + response.StatusCode + " Call Details=" + GetCallDetails(uri));
                }
            }
        }
        private static string GetCallDetails(string uri)
        {
            StringBuilder sb = new StringBuilder();
            var u = new Uri(uri);
            sb.Append(u.AbsolutePath);
            if (u.Query.StartsWith("?"))
            {
                var queryParameters = u.Query.Substring(1).Split('&');
                foreach (var p in queryParameters)
                {
                    if (!(p.ToLower().StartsWith("api") || p.ToLower().StartsWith("nonce")))
                    {
                        var kv = p.Split('=');
                        if (kv.Length == 2)
                        {
                            if (sb.Length != 0)
                            {
                                sb.Append(", ");
                            }

                            sb.Append(kv[0]).Append(" = ").Append(kv[1]);
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }
}
