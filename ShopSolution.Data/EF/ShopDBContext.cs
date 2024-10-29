using Microsoft.EntityFrameworkCore;
using ShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.EF
{
    public class ShopDBContext : DbContext
    {
        public ShopDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Product> Categories { get; set; }
    }
}
