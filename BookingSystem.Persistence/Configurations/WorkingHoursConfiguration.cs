using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Configurations;

public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHours>
{
    public void Configure(EntityTypeBuilder<WorkingHours> builder)
    {
        builder.ToTable("working_hours");

        builder.HasKey(wh => wh.Id);

        builder.Property(wh => wh.ProviderId)
            .IsRequired();

        builder.Property(wh => wh.DayOfWeek)
            .IsRequired();

        builder.Property(wh => wh.StartTime)
            .IsRequired();

        builder.Property(wh => wh.EndTime)
            .IsRequired();

        builder.Property(wh => wh.IsActive)
            .IsRequired();

        // Ensure that each provider can only have one entry per day of the week
        builder.HasIndex(wh => new { wh.ProviderId, wh.DayOfWeek })
            .IsUnique();
    }
}
