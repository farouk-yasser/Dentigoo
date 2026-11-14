using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
    public class CaseFileConfiguration : IEntityTypeConfiguration<CaseFile>
    {
        public void Configure(EntityTypeBuilder<CaseFile> builder)
        {
            builder.HasKey(cf => cf.Id);
            builder.Property(cf => cf.FileName)
                .HasMaxLength(255)
                .IsRequired();
            builder.Property(cf => cf.FileType)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(cf => cf.FileSize)
                .IsRequired();
            builder.Property(cf => cf.StoragePath)
                .IsRequired();
            builder.Property(cf => cf.FileCategory)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(cf => cf.UploadedAt)
                .IsRequired();

            //relation

            builder.HasOne(cf => cf.Patient)
                .WithMany()
                .HasForeignKey(cf => cf.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.DentalCase)
                .WithMany()
                .HasForeignKey(cf => cf.DentalCaseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.Tooth)
                .WithMany()
                .HasForeignKey(cf => cf.ToothId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cf => cf.UploadedByUser)
                .WithMany()
                .HasForeignKey(cf => cf.UploadedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

        }
       
    }
}
