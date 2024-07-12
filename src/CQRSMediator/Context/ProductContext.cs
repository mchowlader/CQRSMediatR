using CQRSMediator.Entities;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator.Context
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions<ProductContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}