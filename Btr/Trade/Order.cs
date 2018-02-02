using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Coin
{
    [DataContract]
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
        [DataMember] public long Id { get; set; }
        [DataMember] public string Pair { get; set; }
        [DataMember] public Double Price { get; set; }
        [DataMember] public Double Amount { get; set; }
        [DataMember] public DateTime ComplitedDate { get; set; }
        [DataMember] public DateTime PlaceDate { get; set; }
    }
}
