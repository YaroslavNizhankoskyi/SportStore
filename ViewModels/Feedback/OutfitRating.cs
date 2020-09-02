using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels.Feedback
{
    public class OutfitRating
    {
        public Outfit Outfit { get; set; }
        public double Rating{ get; set; }
    }
}
