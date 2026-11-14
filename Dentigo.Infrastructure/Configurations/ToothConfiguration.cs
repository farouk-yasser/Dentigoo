using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
   public class ToothConfiguration : IEntityTypeConfiguration<Tooth>
    {
        public void Configure(EntityTypeBuilder<Tooth> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.ToothNumber)
                .HasMaxLength(10)
                .IsRequired();
            builder.Property(t => t.Notes)
                .IsRequired(false);

            //Relation:
            builder.HasOne(t => t.Patient)
                .WithMany().
                HasForeignKey(t => t.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
