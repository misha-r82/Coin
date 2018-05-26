using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coin
{
    public class DbgSett
    {
        static DbgSett()
        {
            Options= new SortedSet<DbgOption>();
            
        }
        public static SortedSet<DbgOption> Options;
        public enum DbgOption
        {
            ShowLoadingHistory,
            ShowCourse,
            ShowBuy,
            ShowSell,
            ShowMGrad,
            ShowUri,
            ApiEmulate,
            ShowTick
            
        } 

    }
}
