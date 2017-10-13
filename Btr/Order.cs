using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Btr
{
    public class Order
    {
        public Order(string pair, double price, double amount)
        {
            Pair = pair;
            Price = price;
            Amount = amount;
        }

        public long Id { get; set; }
        public string Pair { get; set; }
        public Double Price { get; set; }
        public Double Amount { get; set; }
        public DateTime ComplitedDate { get; set; }
    }
}
