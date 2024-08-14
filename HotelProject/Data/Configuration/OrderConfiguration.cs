using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
       .HasOne(o => o.Reservation)
       .WithMany()
       .HasForeignKey(o => o.ReservationId)
       .OnDelete(DeleteBehavior.NoAction);

            builder.Property(o => o.TotalPrice).HasColumnType("decimal(18,2)");
        }
    }
}
