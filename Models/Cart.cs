using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Models
{
    public class Cart
    {
        public List<CartLine> Lines = new List<CartLine>();

        public virtual void AddLine(Outfit outfit, int quantity)
        {
            
            if (Lines.Any(ct => ct.Outfit.Id == outfit.Id))
            {
                Lines.Where(ct => ct.Outfit.Id == outfit.Id).ToList().ForEach(ct => ct.Quantity += 1);
            }
            else
            {
                CartLine line = new CartLine
                {
                    Outfit = outfit,
                    Quantity = quantity
                };

                Lines.Add(line);
            }   
            
        }

        public virtual void RemoveLine(int id)
        {
            Lines.RemoveAll(b => b.Outfit.Id == id);
        }

        public virtual void Clear()
        {
            Lines.Clear();
        }

        public virtual int Sum()
        {
            int sum = 0;
            foreach (CartLine c in Lines)
            {
                int line = c.Outfit.Price * c.Quantity;
                if (c.Outfit.Discount.HasValue)
                {
                    line = line * (100 - c.Outfit.Discount.Value) / 100 ;
                }
                sum += line;
            }
            return sum;
        }

        public virtual void ChangeLine(int OutfitId, int qty)
        {
            Lines.Where(ct => ct.Outfit.Id == OutfitId).ToList().ForEach(ct => ct.Quantity = qty);
        }
    }
    public class CartLine
    {
        public int Price { get; set; }
        public Outfit Outfit { get; set; }
        public int Quantity { get; set; }
    }
}
