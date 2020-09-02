using SportStore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportStore.ViewModels
{
    public class OutfitCreate
    {
        [Required]
        [MinLength(3, ErrorMessage = "Минимальная длина 3")]
        [MaxLength(49, ErrorMessage = "Максимальная длина 50")]
        public string Title { get; set; }

        [Required]
        [Range(10, 50000)]
        public int Price { get; set; }

        [Required]
        public string DepartmentName { get; set; }


        [Range(10,90)]
        public int ?Discount { get; set; }


        [MaxLength(449, ErrorMessage = "Максимальная длина 450 символов")]
        public string Description { get; set; }

        public IFormFile Photo { get; set; }

        
    }
}
