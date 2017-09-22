using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public class DbgSett
    {
        static DbgSett()
        {
            var course = new[] {DbgOption.ShowBuy, DbgOption.ShowSell, DbgOption.ShowCourse};
            Options= new SortedSet<DbgOption>(course);
            
        }
        public static SortedSet<DbgOption> Options;
        public enum DbgOption
        {
            ShowCourse,
            ShowBuy,
            ShowSell

        } 

    }
}
