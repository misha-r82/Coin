using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Lib;
using Newtonsoft.Json;

namespace Btr.History
{
    public class Cryptocompare
    {
        private const string URI_PATT = "https://min-api.cryptocompare.com/data/histominute?fsym={0}&tsym={1}&limit={2}&aggregate=1&e=CCCAGG&toTs={3}";
        private const int LOAD_LIMIT = 3;
        public T CallPublic<T>(string uri)
        {

            if (DbgSett.Options.Contains(DbgSett.DbgOption.ShowUri))
                Debug.WriteLine(uri);
            var request = HttpWebRequest.CreateHttp(uri);
            using (var response = (HttpWebResponse) request.GetResponse())
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
                    throw new Exception("Error - StatusCode=" + response.StatusCode + " Call Details=" + uri);
                }
            }
        }

        public IEnumerable<CourseItem> GetCourse(string coin1, string coin2, DatePeriod period)
        {
            DateTime from = period.From;
            while (from < period.To)
            {
                var fromStamp = Utils.DateTimeToUnixTimeStamp(from);
                string uri = string.Format(URI_PATT, coin1, coin2, LOAD_LIMIT, fromStamp);
                var data = CallPublic<CCResponse>(uri).Data;
                foreach (CCItem item in data)
                {
                    if (period.IsConteins(item.Date))
                        yield return item.CourseItem;
                }
                from = data[data.Length - 1].Date.AddMinutes(1);                
            }
        }
        
        private class CCResponse
        {
            public string Response;
            public CCItem[] Data;
        }
        private class CCItem
        {
            public ulong time;
            public double open;
            public double close;
            public DateTime Date { get { return Utils.UnixTimeStampToDateTime(time); } }

            public CourseItem CourseItem
            {
                get
                {
                    double delta = close - open;
                    double sred = (close + open) / 2;
                    return new CourseItem(Date, sred, delta);
                }
            }
        }
    }
}
