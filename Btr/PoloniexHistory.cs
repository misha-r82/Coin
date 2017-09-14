using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public static class PoloniexHistory
    {
        const string STR_HISTORY = "https://poloniex.com/public?command=returnTradeHistory&currencyPair=BTC_NXT&start={0}&end={1}";
        public void GetHisstory(DateTime from, DateTime to)
        {
            int from = 1;
            int to = 2;
            strring uri = string.Format(STR_HISTORY, from, to);

        }
        
    }
}
