using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Configurations;

public class BlockedTimeConfiguration : IEntityTypeConfiguration<BlockedTime>
{
    public void Configure(EntityTypeBuilder<BlockedTime> builder)
    {
        builder.ToTable("blocked_times");

        builder.HasKey(bt => bt.Id);

        builder.Property(bt => bt.ProviderId)
            .IsRequired();
        builder.Property(bt => bt.StartDate)
            .IsRequired();
        builder.Property(bt => bt.EndDate)
            .IsRequired();
        builder.Property(bt => bt.Reason)
            .HasMaxLength(500);
        builder.Property(bt => bt.CreatedAt)
            .IsRequired();
    }
}
