using SportStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class IndexViewModel
    {
        public List<Outfit> Outfits{ get; set; }
        public string SelectedDepartment { get; set; }
        public string SelectedSort{ get; set; }
        public int SelectedMinPrice { get; set; }
        public int SelectedMaxPrice { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public string OutfitTitle { get; set; }
    }
}
