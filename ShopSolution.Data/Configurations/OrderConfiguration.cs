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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");

            builder.HasKey(x => x.OrderId);

            builder.Property(x => x.OrderId)
                   .IsRequired()
                   .ValueGeneratedNever(); // Không tự động sinh giá trị

            builder.Property(x => x.OrderDate)
                   .IsRequired();

            builder.Property(x => x.Email)
                   .IsRequired()
                   .IsUnicode(false)
                   .HasMaxLength(50);
        }
    }

}
