using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public interface IOutfitRepository
    {
        List<Outfit> GetCart();
        void AddToCart(Outfit outfit);
        void Add(Outfit outfit);
        void AddRange(IEnumerable<Outfit> outfits);
        void Edit(Outfit outfits);
        Outfit Get(int id);
        void Remove(Outfit entity);
        IEnumerable<Outfit> GetAll();

        bool IsEmpty();
    }
}
