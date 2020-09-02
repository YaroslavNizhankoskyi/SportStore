using SportStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public class SQLOrderedOutfitRepository : IOrderedOutfitRepository
    {
        private readonly AppDbContext context;

        public SQLOrderedOutfitRepository(AppDbContext context)
        {
            this.context = context;
        }

        public void AddOrderedOutfits(List<CartLine> Lines, int orderId)
        {
            foreach (CartLine ct in Lines)
            {

                OrderedOutfit ob = new OrderedOutfit
                {
                    OrderId = orderId,
                    OutfitId = ct.Outfit.Id,
                    Quantity = ct.Quantity
                };


                context.OrderedOutfits.Add(ob);
            }
            context.SaveChanges();


        }

        public IList<OrderedOutfit> GetOrderedOutfits(int orderId)
        {
            return context.OrderedOutfits.Where(b => b.OrderId == orderId).ToList();
        }

        public void RemoveOrderedOutfits(int orderId)
        {
            var orderedOutfits = context.OrderedOutfits.Where(b => b.OrderId == orderId);
            foreach (OrderedOutfit b in orderedOutfits) context.OrderedOutfits.Remove(b);
            context.SaveChanges();
        }

        public List<int> TopOutfitForUser(string userId)
        {
            List<int> TopForUser;
            //User's orders
            var orderIds = context.Orders.Where(o => o.Customer.Id == userId).Select(o => o.Id);

            //In case no orders - null
            if (!orderIds.Any()) return null;

            //Find ordered Outfits
            List<OrderedOutfit> orderedOutfits = new List<OrderedOutfit>();
            foreach (var o in context.OrderedOutfits.ToList())
            {
                if (orderIds.Contains(o.OrderId)) orderedOutfits.Add(o);
            }
            
            //Ordered Outfit's id
            var orderedOutfitIds = orderedOutfits.Select(o => o.OutfitId);

            //User feedbacks
            var feedbacks = from f in context.Feedbacks.ToList()
                            where f.CustomerId == userId &&
                            f.Rating >= 2
                            select f.OutfitId;

            if (feedbacks.Any())
            {
                
                TopForUser = orderedOutfitIds.Intersect(feedbacks).ToList();
                //Return ordered Outfit ids which customer  has commented with rating more than 2
                if (TopForUser.Any()) return TopForUser ;
                //else return ordered Outfits ids 
                else return orderedOutfitIds.ToList();
            }


            return orderedOutfitIds.ToList();
        }

        //QUERY ORDERED OutfitS
        public IEnumerable<OrderQuantity> PopularOutfits()
        {
            var Outfits = from o in context.OrderedOutfits.ToList()
                        group o by o.OutfitId into g 
                        select new OrderQuantity 
                        {
                            Outfit = context.Outfits.FirstOrDefault(b => b.Id == g.Key),
                            OutfitQuantity = g.Sum(o => o.Quantity)
                        };

            return Outfits;          
        }
        public void RemoveOrdersByOutfits(int id)
        {
            var orders = from o in context.OrderedOutfits.ToList()
                     where o.OutfitId == id
                     select o;
            context.OrderedOutfits.RemoveRange(orders);
            context.SaveChanges();
        }
    }


    public class OrderQuantity
    {
        public Outfit Outfit { get; set; }
        public int OutfitQuantity { get; set; }
    }


}
