using Microsoft.AspNetCore.Identity;
using SportStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public class SQLOrderRepository : IOrderRepository
    {
        private readonly AppDbContext context;

        public SQLOrderRepository(AppDbContext context)
        {
            this.context = context;

        }

        public Order AddOrder(Cart cart, Customer customer)
        {
            Order order = new Order
            {
                Customer = customer,
                Price = cart.Sum(),
            };
            context.Orders.Add(order);
            context.SaveChanges();
            return order;
        }

        public void EditOrder(Order order)
        {
            var Order = context.Orders.Attach(order);
            Order.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        public Order GetOrder(int orderId)
        {
            return context.Orders.FirstOrDefault(o => o.Id == orderId);
        }

        public IEnumerable<Order> GetOrders(string userId)
        {
            return context.Orders.Where(o => o.Customer.Id ==  userId);
        }

        public Order RemoveOrder(int orderId)
        {
            var order = GetOrder(orderId);
            context.Orders.Remove(order);
            context.SaveChanges();
            return order;
        }
    }
}
