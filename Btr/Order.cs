using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public class Order
    {
        public long Id { get; set; }
        public DateTime BuyDate { get; set; }
        public DateTime SellDate { get; set; }
        public Market Market { get; set; }
    }
}
