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

namespace Coin.History
{
    public class Cryptocompare
    {
        private const string URI_PATT = "https://min-api.cryptocompare.com/data/histominute?fsym={0}&tsym={1}&aggregate=1&e=CCCAGG&toTs={2}&limit={3}";
        private TimeSpan _wnd = new TimeSpan(0,1,0);
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
            DateTime to = period.To;
            while (from < period.To)
            {
                to = from + _wnd;
                if (to > period.To) to = period.To;
                int remainMin = (period.To - from).Minutes;
                int limit = remainMin < _wnd.Minutes ? _wnd.Minutes : remainMin;
                to = new DateTime(2017, 11, 7, 12, 38, 00);
                var toStamp = Utils.DateTimeToUnixTimeStamp(to);
                string uri = string.Format(URI_PATT, coin1, coin2, toStamp, limit);
                var data = CallPublic<CCResponse>(uri).Data;
                foreach (CCItem item in data)
                {
                    if (period.IsConteins(item.Date))
                        yield return item.CourseItem;
                }
                from.AddMinutes(limit + 1);                
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
                    return new CourseItem(Date, sred, delta, 0);
                }
            }
        }
    }
}
