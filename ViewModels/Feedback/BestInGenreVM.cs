using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels.Feedback
{
    public class BestInGenreVM
    {
        public IEnumerable<BestInGenre> Outfits{ get; set; }
        public SortViewModel SortModel { get; set; }
        public string ReturnUrl { get; set; }
    }
}
