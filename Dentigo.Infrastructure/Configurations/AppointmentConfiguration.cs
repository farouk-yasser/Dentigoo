using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
    public class AppointmentConfiguration
     : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.VisitReason)
                .HasConversion<string>()
                .IsRequired();
            builder.Property(a => a.Status)
                .HasConversion<string>()
                .IsRequired();
            builder.Property(a => a.Notes)
               .HasMaxLength(500);
            builder.Property(a => a.CreatedAt)
                .HasDefaultValueSql("GETDATE()");
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(a => new
            {
                a.DoctorId,
                a.AppointmentDate,
                a.AppointmentTime
            })
            .HasFilter("[Status] = 'Scheduled'")
            .IsUnique();
        }
    }
}
