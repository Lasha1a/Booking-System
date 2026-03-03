using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Configurations;

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.ToTable("service_providers");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.Specialty)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.IsActive)
            .IsRequired();

        builder.HasIndex(sp => sp.Email)
            .IsUnique();

        builder.HasMany(sp => sp.Appointments)
            .WithOne(a => a.Provider)
            .HasForeignKey(a => a.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.WorkingHours)
            .WithOne(wh => wh.Provider)
            .HasForeignKey(wh => wh.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(sp => sp.BlockedTimes)
            .WithOne(bt => bt.Provider)
            .HasForeignKey(bt => bt.ProviderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
