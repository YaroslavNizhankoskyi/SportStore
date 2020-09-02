using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels.Feedback
{
    public class FeedbackVM
    {
       
        public string UserName { get; set; }
        [Required(ErrorMessage ="Try again")]
        public int OutfitId { get; set; }
        [Required(ErrorMessage ="Rating is required")]
        [Range(0,5)]
        public int Rating { get; set; }
        public string Comment { get; set; }
        public bool IsUsers { get; set; }
        [Required]
        public int CommentId { get; set; }
    }
}
