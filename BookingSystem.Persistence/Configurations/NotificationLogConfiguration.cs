using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.ToTable("notification_logs");

        builder.HasKey(nl => nl.Id);

        builder.Property(nl => nl.AppointmentId)
            .IsRequired();
        builder.Property(nl => nl.NotificationType)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(nl => nl.SentAt)
            .IsRequired();

        // Relationship to Appointment
        builder.HasOne(nl => nl.Appointment)
            .WithMany(a => a.NotificationLogs)
            .HasForeignKey(nl => nl.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
