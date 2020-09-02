using SportStore.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SportStore.Models.Repositories
{
    public class SqlOutfitRepository : IOutfitRepository
    {
        private AppDbContext context;
       
        public List<Outfit> Cart = new List<Outfit>();

        public List<Outfit> GetCart()
        {
            return Cart;
        }
        public void AddToCart(Outfit Outfit)
        {
            Cart.Add(Outfit);
        }

        public SqlOutfitRepository(AppDbContext context)
        {
            this.context = context;
        }
        public void Add(Outfit entity)
        {
            context.Outfits.Add(entity);
            context.SaveChanges();
        }

        public void AddRange(IEnumerable<Outfit> entities)
        {
            throw new NotImplementedException();
        }

        public void Edit(Outfit entity)
        {
            var Outfit = context.Outfits.Attach(entity);
            Outfit.State = EntityState.Modified;
            context.SaveChanges();
        }

        public IEnumerable<Outfit> Find(Expression<Func<Outfit, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Outfit Get(int id)
        {
            return context.Outfits.FirstOrDefault(Outfit => Outfit.Id == id);
        }

        public IEnumerable<Outfit> GetAll()
        {
            return context.Outfits;
        }

        public void Remove(Outfit entity)
        {
            context.Outfits.Remove(entity);
            context.SaveChanges();
        }


        public List<Outfit> FindSimilarDepartment(string Department, int used)
        {
            
            IEnumerable<Outfit> similarOutfits = from b in context.Outfits.ToList()
                                      where b.DepartmentName == Department && b.Id != used
                                      select b;

            
            return similarOutfits.ToList();
        }

        public bool IsEmpty()
        {
            if (context.Outfits == null || !context.Outfits.Any()) return true;
            else return false;
        }
    }
}
