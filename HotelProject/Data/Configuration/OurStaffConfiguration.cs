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
    internal class OurStaffConfiguration : IEntityTypeConfiguration<OurStaff>
    {
        public void Configure(EntityTypeBuilder<OurStaff> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired(true);
            builder.Property(x=>x.Position).HasMaxLength(50).IsRequired(true);
        }
    }
}
