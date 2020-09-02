using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels.Feedback
{
    public class TopRatedOutfitsVM
    {
        public IEnumerable<RatedOutfit> RatedOutfits{ get; set; }
        public SortViewModel SortViewModel { get; set; }
        public string ReturnUrl { get; set; }
    }
}
