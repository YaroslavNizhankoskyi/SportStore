using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class RegisterViewModel
    {

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
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        [MinLength(6, ErrorMessage = "Минимум 6 знаков")]
        [MaxLength(20, ErrorMessage = "Максимум 20 знаков")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Город")]
        [MaxLength(50, ErrorMessage = "Слишком длинное имя города")]
        [MinLength(3, ErrorMessage = "Слишком короткое имя города")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Подтверждение пароля")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string Role { get; set; }

    }
}
