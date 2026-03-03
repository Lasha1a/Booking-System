using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.ProviderId)
            .IsRequired();
        builder.Property(a => a.CustomerName)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.CustomerEmail)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.CustomerPhone)
            .IsRequired()
            .HasMaxLength(20);
        builder.Property(a => a.AppointmentDate)
            .IsRequired();
        builder.Property(a => a.StartTime)
            .IsRequired();
        builder.Property(a => a.EndTime)
            .IsRequired();
        builder.Property(a => a.Status)
            .IsRequired()
            .HasConversion<string>();
        builder.Property(a => a.CancellationReason)
            .HasMaxLength(500);
        builder.Property(a => a.IsRecurring)
            .IsRequired();
        builder.Property(a => a.ReccurenceRule)
            .HasMaxLength(100);
        builder.Property(a => a.CreatedAt)
            .IsRequired();
        builder.Property(a => a.UpdatedAt)
            .IsRequired();

        // Self referencing relationship for recurring appointments
        builder.HasOne(a => a.ParentAppointment)
            .WithMany(a => a.ChildAppointments)
            .HasForeignKey(a => a.ParentAppointmentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        // Relationship to NotificationLogs
        builder.HasMany(a => a.NotificationLogs)
            .WithOne(nl => nl.Appointment)
            .HasForeignKey(nl => nl.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
