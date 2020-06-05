using System.Threading.Tasks;
using Coin.Data;
using Lib;

namespace Coin
{
    public interface IApiDriver
    {
        string Name { get; }
        ApiWebBase Api { get; }
        Task Buy(Order order);
        Task Sell(Order order);
        Task CanselOrder(Order order);
        Task<Order[]> OrderHistory(string pair, DatePeriod period);
        Task<bool> IsComplited(Order order);
        HistoryItem[] GetHitory(string market, DatePeriod period);
    }
}