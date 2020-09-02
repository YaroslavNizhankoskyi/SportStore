using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.ViewModels
{
    public class OrderVM
    {
        public OrderVM()
        {

            OrderId = 0;
            Lines = new List<CartLine>();
            OrderPrice = 0;
            Number = 0;
        }
        public int OrderId { get; set; }
        public List<CartLine> Lines { get; set; }

        public int OrderPrice { get; set; }

        public int Number { get; set; }
    }

}
