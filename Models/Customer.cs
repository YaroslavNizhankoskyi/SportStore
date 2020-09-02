using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models
{
    
    public class Customer : IdentityUser
    {
        [Required]
        [Display(Name ="Город")]
        [StringLength(50)]
        public string City { get; set; }

        //Navigation
        public virtual ICollection<Order> Orders { get; set; }
    }
}
