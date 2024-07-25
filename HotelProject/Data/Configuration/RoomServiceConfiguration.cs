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
    internal class RoomServiceConfiguration : IEntityTypeConfiguration<RoomService>
    {
        public void Configure(EntityTypeBuilder<RoomService> builder)
        {
            builder.HasKey(x => new { x.RoomId,x.ServiceId });
            builder.HasOne(x=>x.Room).WithMany(z=>z.RoomServices).HasForeignKey(x=>x.RoomId);
            builder.HasOne(x => x.Service).WithMany(z => z.RoomServices).HasForeignKey(x => x.ServiceId);
        }
    }
}
