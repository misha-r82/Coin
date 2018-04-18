using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Coin.Data
{
    public class HistoryItem
    {
        //public int tradeId, globalTradeID;
        public DateTime date;
        public string type;
        public double rate, amount, total;
    }
}
