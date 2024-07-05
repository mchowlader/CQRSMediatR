using CQRSMediator.Models;
using Microsoft.EntityFrameworkCore;

namespace CQRSMediator
{
    public class ProductContext : DbContext
    {
        public ProductContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }
}