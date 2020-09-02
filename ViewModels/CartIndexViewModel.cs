using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class CartIndexViewModel
    {
        public Cart Cart { get; set; }
        public string ReturnUrl { get; set; }
        public int OrderId { get; set; }
        public CartIndexViewModel()
        {
            Cart = new Cart();
            ReturnUrl = string.Empty;
            OrderId = 0;
        }
    }
}
