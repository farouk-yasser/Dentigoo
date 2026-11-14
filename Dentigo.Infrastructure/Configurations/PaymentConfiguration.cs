using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dentigo.Infrastructure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.PaymentDate)
                .IsRequired();

            builder.Property(p => p.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.PaymentStatus)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.DentalCase)
                .WithMany()
                .HasForeignKey(p => p.DentalCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(p => p.RecordedByUser)
                .WithMany()
                .HasForeignKey(p => p.RecordedByUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(p => p.TreatmentPlan)
           .WithMany()
           .HasForeignKey(p => p.TreatmentPlanId)
           .IsRequired(false)
           .OnDelete(DeleteBehavior.NoAction);
           
        }
    }
}