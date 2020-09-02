using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models { 

    public class Outfit
    {
        
        public int Id { get; set; }

        [Display(Name = "Название")]
        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [ForeignKey("Department")]
        [Required]
        public string DepartmentName { get; set; }

        [Display(Name = "Цена")]
        [Required]
        [Range(50, 100000)]
        public int Price { get; set; }

        [Display(Name="Описание")]
        [StringLength(400)]
        public string Description { get; set; }

        [Range(1, 90)]
        public int? Discount { get; set; }

        public string PhotoPath { get; set; }

        public Department Department { get; set; }




    }
}
