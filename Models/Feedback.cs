using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }

        public string Comment { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        public int OutfitId { get; set; }

        [Required]
        public string CustomerId { get; set; }

        [Required]
        public DateTime Date{ get; set; }



        //Navigation
        public Outfit Outfit { get; set; }
        public Customer Customer { get; set; }
    }
}
