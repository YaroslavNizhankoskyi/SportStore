using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public interface IOrderedOutfitRepository
    {
        void AddOrderedOutfits(List<CartLine> Lines, int orderId);
        void RemoveOrderedOutfits(int orderId);
        IList<OrderedOutfit> GetOrderedOutfits(int orderId);
        IEnumerable<OrderQuantity> PopularOutfits();
        List<int> TopOutfitForUser(string userId);
        void RemoveOrdersByOutfits(int id);
    }
}
