using Dentigo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dentigo.Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.NotificationType)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(n => n.Message)
                .IsRequired();
            builder.Property(n => n.CreatedAt)
                .IsRequired();
            builder.Property(n => n.IsRead)
                .IsRequired();

            //relation

            builder.HasOne(n => n.RecipientUser)
                .WithMany()
                .HasForeignKey(n => n.RecipientUserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(n => n.DentalCase)
                .WithMany()
                .HasForeignKey(n => n.DentalCaseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}