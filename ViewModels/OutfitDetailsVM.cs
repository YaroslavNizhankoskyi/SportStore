using SportStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class OutfitDetailsVM
    {
        public int Id { get; set; }
        [Display(Name = "Название")]
        public string Title { get; set; }

        public string Department { get; set; }


        [Display(Name = "Цена")]
        public int Price { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        public int? Discount { get; set; }

        public string PhotoPath { get; set; }
    }
}
