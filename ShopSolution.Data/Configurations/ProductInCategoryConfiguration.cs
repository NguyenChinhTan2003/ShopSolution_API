using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopSolution.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Configurations
{
    public class ProductInCategoryConfiguration : IEntityTypeConfiguration<ProductInCategory>
    {
        public void Configure(EntityTypeBuilder<ProductInCategory> builder)
        {
            builder.HasKey(t => new { t.ProductId, t.CategoryId });

            builder.ToTable("ProductInCategories");


            builder.HasOne(t => t.Product).WithMany(pc => pc.ProductInCategories).HasForeignKey(pc => pc.ProductId);

            builder.HasOne(t => t.Category).WithMany(pc => pc.ProductInCategories).HasForeignKey(pc => pc.CategoryId);
        }
    }
}
