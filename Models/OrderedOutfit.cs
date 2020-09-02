using SportStore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models
{
    public class OrderedOutfit
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        public int OutfitId { get; set; }
        [Required]
        public int OrderId { get; set; }
        //Navigation
        public Outfit Outfit{ get; set; }
        public Order Order { get; set; }
    }
}
