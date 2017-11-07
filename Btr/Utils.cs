using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public class Utils
    {
        public static DateTime UnixTimeStampToDateTime(ulong unixTimeStamp)
        {
            return PlnHistory.DateTimeUnixEpochStart.AddSeconds(unixTimeStamp);
        }

        public static ulong DateTimeToUnixTimeStamp(DateTime dateTime)
        {
            return (ulong)Math.Floor(dateTime.Subtract(PlnHistory.DateTimeUnixEpochStart).TotalSeconds);
        }
    }
}
