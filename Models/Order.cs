using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Цена")]
        public int Price { get; set; }

        //Navigation
        public virtual Customer Customer { get; set; }
        public virtual ICollection<OrderedOutfit> OrderedBooks { get; set; }
    }
}
