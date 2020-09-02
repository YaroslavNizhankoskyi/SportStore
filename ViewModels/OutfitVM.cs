using SportStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class OutfitsVM
    {
        public List<Outfit> Outfits  { get; set; }
        public SortViewModel SortViewModel { get; set; }
        public string SelectedDepartment { get; set; }
        public int SelectedPrice { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
    }
}
