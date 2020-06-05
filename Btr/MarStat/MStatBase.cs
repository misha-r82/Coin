using Coin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr.MarStat
{
    public class MStatBase
    {
        public Market Market { get; }
        public MStatBase(Market market)
        {
            Market = market;
        }

        public string Name
        {
            get { return Market.Name; }
        }
        public DateTime MinDate
        {
            get
            {
                if (Market.CourseData == null) return new DateTime();
                return Market.CourseData[0].date;
            }
        }
        public DateTime MaxDate
        {
            get
            {
                if (Market.CourseData == null) return new DateTime();
                return Market.LastPt.Date;
            }
        }
        
    }
}
