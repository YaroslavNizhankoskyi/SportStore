using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class ProfileViewModel
    {

      
        public string Id { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Слишком длинное имя")]
        [MinLength(5, ErrorMessage = "Слишком короткое имя")]
        [Display(Name = "Ваше имя")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailInUse", controller: "Account")]
        [Display(Name = "Почта")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Город")]
        [MaxLength(50, ErrorMessage = "Слишком длинное имя города")]
        [MinLength(3, ErrorMessage = "Слишком короткое имя города")]
        public string Location { get; set; }
    }
}
