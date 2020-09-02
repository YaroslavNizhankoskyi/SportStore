using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders(string userId);
        Order GetOrder(int orderId);
        Order AddOrder(Cart cart, Customer customer);
        Order RemoveOrder(int orderId);
        void EditOrder(Order order);
    }
}
