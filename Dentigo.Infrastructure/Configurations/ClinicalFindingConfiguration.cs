using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
    public class ClinicalFindingConfiguration : IEntityTypeConfiguration<ClinicalFinding>
    {
        public void Configure(EntityTypeBuilder<ClinicalFinding> builder)
        {
            builder.HasKey(cf => cf.Id);
            builder.Property(cf => cf.Description)
                .IsRequired();
            builder.Property(cf => cf.CreatedAt)
                .IsRequired();
            builder.Property(cf => cf.UpdatedAt)
                .IsRequired(false);

            //relation

            builder.HasOne(cf => cf.Patient)
                .WithMany().
                HasForeignKey(cf => cf.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.Tooth)
                .WithMany()
                .HasForeignKey(cf => cf.ToothId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.DentalCase)
                .WithMany()
                .HasForeignKey(cf => cf.DentalCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.CreatedByUser)
                .WithMany()
                .HasForeignKey(cf => cf.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
