using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Configuration
{
    internal class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(true);
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Price).IsRequired(true);
            builder.Property(x=>x.Area).IsRequired(true);
            builder.Property(x=>x.MaxChildrenCount).IsRequired(true);
            builder.Property(x=>x.MaxAdultsCount).IsRequired(true);

            builder.HasOne(x => x.BedType)
               .WithMany(x => x.Room)
               .HasForeignKey(x => x.BedTypeId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Branch)
                .WithMany(b => b.Rooms) 
                .HasForeignKey(x => x.BranchId);
        }
    }
}
