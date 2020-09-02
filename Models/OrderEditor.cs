using System.Collections.Generic;
using System.Linq;

namespace SportStore.Models
{
    public class OrderEditor
    {
        public List<CartLine> Lines = new List<CartLine>();

        public virtual void AddLine(Outfit outfit, int quantity)
        {

            if (Lines.Any(ct => ct.Outfit.Id == outfit.Id))
            {
                Lines.Where(ct => ct.Outfit.Id == outfit.Id).ToList().ForEach(ct => ct.Quantity += quantity);
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
                if (c.Outfit.Discount > 0)
                {
                    line *= c.Outfit.Discount.Value;
                }
                sum += line;
            }
            return sum;
        }

        public virtual void ChangeLine(int outfitId, int qty)
        {
            Lines.Where(ct => ct.Outfit.Id == outfitId).ToList().ForEach(ct => ct.Quantity = qty);
        }
    }

}

