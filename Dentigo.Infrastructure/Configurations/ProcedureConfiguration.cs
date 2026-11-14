using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
namespace Dentigo.Infrastructure.Configurations
{
   public class ProcedureConfiguration : IEntityTypeConfiguration<Procedure>
    {
        public void Configure(EntityTypeBuilder<Procedure> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();
            builder.Property(p => p.Description)
                .IsRequired(false);
            builder.Property(p => p.SequenceNumber)
                .IsRequired();

            //relation

            builder.HasOne(p => p.TreatmentPlan)
                .WithMany()
                .HasForeignKey(p => p.TreatmentPlanId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.DentalCase)
                .WithMany()
                .HasForeignKey(p => p.DentalCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}
