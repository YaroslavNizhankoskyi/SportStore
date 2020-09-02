using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportStore.Data
{
    public class AppDbContext : IdentityDbContext<Customer>
    {
        public DbSet<Outfit> Outfits{ get; set; }
        public DbSet<Order> Orders{ get; set; }
        public DbSet<OrderedOutfit> OrderedOutfits { get; set; }
        public DbSet<Feedback> Feedbacks{ get; set; }

        public DbSet<Department> Departments { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            
        }
    }
}
