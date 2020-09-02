using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public interface IFeedbackRepository
    {
        List<Feedback> GetFeedbacks(int outfitId);
        void AddFeedback(Feedback fb);
        void RemoveFeedback(int feedId);
        double OutfitRating(int outfitId);
        List<RatedOutfit> BestRatedOutfits();
        IEnumerable<BestInGenre> BestInGenre();
        void RemoveOutfitFeedback(int id);
    }
}
