using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class UserOrdersVM
    {
        
        public List<OrderVM> Orders { get; set; }
        public int Price { get; set; }
    }
}
