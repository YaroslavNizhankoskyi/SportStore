
using SportStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SportStore.Models.Repositories
{
    public class SQLFeedbackRepository : IFeedbackRepository
    {
        public AppDbContext Context { get; }

        public SQLFeedbackRepository(AppDbContext context)
        {
            Context = context;
        }

        public void AddFeedback(Feedback fb)
        {
            Context.Feedbacks.Add(fb);
            Context.SaveChanges();
        }


        //QUERY Outfit RATING 
        public List<RatedOutfit> BestRatedOutfits()
        {
            List<RatedOutfit> Ratings = new List<RatedOutfit>();

            //filling ratings 
            Context.Outfits.ToList().ForEach(b => Ratings.Add(
                new RatedOutfit
                {
                    Outfit = b,
                    Rating = OutfitRating(b.Id)
                }));
            //sort ratings
            return Ratings.OrderBy(b => b.Rating).ToList(); 

        }

        public double OutfitRating(int OutfitId)
        {
            double rating = 0;
            int count = 0;
            var feedbacks = from f in Context.Feedbacks
                            where f.OutfitId == OutfitId
                            select f.Rating;
            if (feedbacks.Any())
            {
                foreach (var f in feedbacks) rating += f;
                count = feedbacks.ToList().Count;
                rating = Math.Round((double)rating / count, 1);
            }
            return rating;
        }

        public List<Feedback> GetFeedbacks(int OutfitId)
        {
            var feedbacks = Context.Feedbacks.Where(f => f.OutfitId == OutfitId).ToList();
            return feedbacks;
        }

        public void RemoveFeedback(int feedId)
        {
            var feed = Context.Feedbacks.Where(f => f.Id == feedId);
            foreach (Feedback f in feed) Context.Feedbacks.Remove(f);
            Context.SaveChanges();
        }

        public IEnumerable<BestInGenre> BestInGenre()
        {
            var Outfit = from b in Context.Outfits.ToList()
                        group b by new { b.DepartmentName, b.Title } into g
                        select new BestInGenre{
                            Title = g.Key.Title,
                            Rating = g.Max(b => OutfitRating(b.Id)),
                            DepartmentName = g.Key.DepartmentName};
            return Outfit;

        }

        public void RemoveOutfitFeedback(int id)
        {

            var feedbacks = from f in Context.Feedbacks.ToList()
                            where f.OutfitId == id
                            select f;

            Context.Feedbacks.RemoveRange(feedbacks);

            Context.SaveChanges();
        }


    }
}
