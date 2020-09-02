using SportStore.Models;
using SportStore.ViewModels.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class OutfitDetailsViewModel
    {
        public Outfit Outfit{ get; set; }
        public List<FeedbackVM> Feedbacks{ get; set; }
        public double Rating { get; set; }
    }
}
