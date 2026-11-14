using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
    public class CaseHistoryEventConfiguration:IEntityTypeConfiguration<CaseHistoryEvent>
    {
        public void Configure(EntityTypeBuilder<CaseHistoryEvent> builder)
        {
            builder.HasKey(he => he.Id);

            builder.Property(he => he.EventType)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(he => he.Description)
                .IsRequired();

            builder.Property(he => he.PreviousStatus)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(he => he.NewStatus)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(he => he.Reason)
                .IsRequired(false);

            builder.Property(he => he.CreatedAt)
                .IsRequired();

            //relation
            builder.HasOne(he => he.DentalCase)
                .WithMany()
                .HasForeignKey(he => he.DentalCaseId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(he => he.ActorUser)
                .WithMany()
                .HasForeignKey(he => he.ActorUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(he => he.RelatedFile)
                .WithMany()
                .HasForeignKey(he => he.RelatedFileId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(he => he.RelatedDesignVersion)
                .WithMany()
                .HasForeignKey(he => he.RelatedDesignVersionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);



        }

    }
}
