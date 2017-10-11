using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BethanysPieShop.Models
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }
        
        public DbSet<Category> Categories { get; set; }

        public DbSet<Pie> Pies { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BethaysPieShop;
                                            Integrated Security=True;Connect Timeout=30;Encrypt=False;
                                            TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }
    }

    public class AppDbContextFactory : IDbContextFactory<AppDbContext>
    {
        public AppDbContext Create(DbContextFactoryOptions options)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            return new AppDbContext(optionsBuilder.Options);
        }
    }

}
