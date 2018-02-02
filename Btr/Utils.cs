using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coin
{
    public class Utils
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            return epoch.AddSeconds(unixTimeStamp);
        }

        public static ulong DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (ulong)Math.Floor(dateTime.Subtract(epoch).TotalSeconds);
        }
    }
}
