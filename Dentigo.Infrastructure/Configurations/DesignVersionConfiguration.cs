using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
   public class DesignVersionConfiguration : IEntityTypeConfiguration<DesignVersion>
    {
        public void Configure(EntityTypeBuilder<DesignVersion> builder)
        {
            builder.HasKey(dv => dv.Id);
            builder.Property(dv => dv.VersionNumber)
                .IsRequired();
            builder.Property(dv => dv.ReviewStatus)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(dv => dv.SubmittedAt)
                .IsRequired();
            builder.Property(dv => dv.ReviewReason)
                .IsRequired(false);

            builder.HasIndex(dv =>
            new { dv.DentalCaseId, dv.VersionNumber })
                .IsUnique();

            builder.HasIndex(dv => 
            dv.CaseFileId)
            .IsUnique();

            //relation

            builder.HasOne(dv => dv.CaseFile)
                .WithMany()
                .HasForeignKey(dv => dv.CaseFileId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dv => dv.SubmittedByUser)
                .WithMany()
                .HasForeignKey(dv => dv.SubmittedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);



        }

    }
}
