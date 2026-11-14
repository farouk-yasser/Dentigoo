using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Dentigo.Infrastructure.Configurations
{
    public class DentalCaseConfiguration : IEntityTypeConfiguration<DentalCase>
    {
        public void Configure(EntityTypeBuilder<DentalCase> builder)
        {
            builder.HasKey(dc => dc.Id);
            builder.Property(dc => dc.TreatmentName)
                .HasMaxLength(150)
                .IsRequired();
            builder.Property(dc => dc.Description)
                .IsRequired(false);
            builder.Property(dc => dc.DoctorInstructions)
                .IsRequired(false);
            builder.Property(dc => dc.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(dc => dc.RequiresRadiology)
                .IsRequired();
            builder.Property(dc => dc.ExpectedDueDate)
                .IsRequired(false);
            builder.Property(dc => dc.CreatedAt)
                .IsRequired();
            builder.Property(dc => dc.UpdatedAt)
                .IsRequired(false);
            builder.Property(dc => dc.CancellationReason)
                .IsRequired(false);

           //relation

            builder.HasOne(dc => dc.Patient)
                .WithMany()
                .HasForeignKey(dc => dc.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dc => dc.Tooth)
                .WithMany()
                .HasForeignKey(dc => dc.ToothId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dc => dc.CreatedByUser)
                .WithMany()
                .HasForeignKey(dc => dc.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
