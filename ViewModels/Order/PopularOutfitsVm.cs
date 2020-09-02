using SportStore.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels.Order
{
    public class PopularOutfitsVm
    {
        public IEnumerable<OrderQuantity> OrderedOutfits { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public string ReturnUrl { get; set; }
    }
}
