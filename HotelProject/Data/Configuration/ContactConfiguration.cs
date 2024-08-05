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
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FullName)
                .HasMaxLength(50)
                .IsRequired(true);

            builder.Property(x => x.Message)
                .HasMaxLength(500) 
                .IsRequired(true);

            builder.Property(x => x.Subject)
                .HasMaxLength(100) 
                .IsRequired(true);

            builder.Property(x => x.Email)
                .HasMaxLength(100)
                .IsRequired(true);

            builder.HasOne(x => x.AppUser)
                .WithMany() 
                .HasForeignKey(x => x.AppUserId)
                .IsRequired();
        }
    }
}
