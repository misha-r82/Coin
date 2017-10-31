using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bittrex;
using Btr.Data;
using Lib;

namespace Btr
{
    public class BtrHistory
    {
        /*            '60' => 'oneMin',
            '300' => 'fiveMin',
            // fifteenMin
            '1800' => 'thirtyMin',
            '3600' => 'hour',
            '86400' => 'day',*/
        const string URI_BTR_PATT = "https://bittrex.com/Api/v2.0/pub/market/GetTicks?marketName={0}&tickInterval=fiveMin&_={1}";
        const string URI_PLN_PATT = "https://poloniex.com/public?command=returnTradeHistory&currencyPair={0}&start={1}&end={2}";
        static BtrHistory()
        {
            ApiCall = new ApiCall(false);
        }
        public static ApiCall ApiCall { get; }
        internal static readonly DateTime DateTimeUnixEpochStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public static BtrHistoryItem[] GetHitoryBtr(string market, DateTime from)
        {
            
            ulong fromStamp = Utils.DateTimeToUnixTimeStamp(from)*1000;
            var uri = string.Format(URI_BTR_PATT, market, fromStamp);
            return ApiCall.CallWithJsonResponse<BtrHistoryItem[]>(uri, false);

        }
        public static PlnHistoryItem[] GetHitoryPln(string market, DatePeriod period)
        {
            ulong fromStamp = Utils.DateTimeToUnixTimeStamp(period.From);
            ulong toStamp = Utils.DateTimeToUnixTimeStamp(period.To);
            var uri = string.Format(URI_PLN_PATT, market, fromStamp, toStamp);
            PlnHistoryItem[] result;
            int max_attempt = 20;
            int attempts = 0;
            try
            {
                result= ApiCall.CallWithJsonResponse<PlnHistoryItem[]>(uri);
            }
            catch (Exception e)
            {
                Thread.Sleep(500);
                if (attempts++ > max_attempt) throw new Exception("не удалось получить данные курса", e);
                return GetHitoryPln(market, period);
            }
            return result;

        }
    }
}
