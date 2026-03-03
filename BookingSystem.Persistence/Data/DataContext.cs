using BookingSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingSystem.Persistence.Data;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public virtual DbSet<Appointment> Appointments { get; set; } 
    public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }
    public virtual DbSet<WorkingHours> WorkingHours { get; set; }
    public virtual DbSet<BlockedTime> BlockedTimes { get; set; }
    public virtual DbSet<NotificationLog> NotificationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
