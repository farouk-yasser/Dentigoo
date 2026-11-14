using Dentigo.Domain.Entities;
using Dentigo.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dentigo.Infrastructure.Configurations
{
    public class TreatmentPlanConfiguration : IEntityTypeConfiguration<TreatmentPlan>
    {
        public void Configure(EntityTypeBuilder<TreatmentPlan> builder)
        {
            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.Name)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(tp => tp.Description)
                .IsRequired(false);

            builder.Property(tp => tp.CreatedAt)
                .IsRequired();

            builder.Property(tp => tp.AgreedCost)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(tp => tp.treatmentPlanStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Relationships

            builder.HasOne(tp => tp.Patient)
                .WithMany()
                .HasForeignKey(tp => tp.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tp => tp.CreatedByUser)
                .WithMany()
                .HasForeignKey(tp => tp.CreatedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}