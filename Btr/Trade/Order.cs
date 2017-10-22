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
        public bool IsComplited { get { return ComplitedDate > new DateTime(0); } }
        public CoursePoint Point { get { return new CoursePoint(Price, PlaceDate);} }
        public long Id { get; set; }
        public string Pair { get; set; }
        public Double Price { get; set; }
        public Double Amount { get; set; }
        public DateTime ComplitedDate { get; set; }
        public DateTime PlaceDate { get; set; }
    }
}
