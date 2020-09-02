using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class BestForUserVm
    {
        public string DepartmentName { get; set; }
        public List<Outfit> OutfitDept{ get; set; }
    }
}
