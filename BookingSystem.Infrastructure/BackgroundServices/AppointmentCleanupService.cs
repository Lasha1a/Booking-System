using BookingSystem.Application.Interfaces.GenericRepo;
using BookingSystem.Domain.Entities;
using BookingSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;


namespace BookingSystem.Infrastructure.BackgroundServices;

public class AppointmentCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AppointmentCleanupService> _logger;

    public AppointmentCleanupService(
        IServiceScopeFactory scopeFactory,
        ILogger<AppointmentCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CleanupOldAppointmentsAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error occurred during appointment cleanup");
            }

            //runs every hour
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private async Task CleanupOldAppointmentsAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IGenericRepository<Appointment>>();

        var cutoffTime = DateTime.UtcNow.AddHours(-2);

        var oldAppointments = await repository.GetAll()
           .Where(a => a.AppointmentDate < cutoffTime
               && (a.Status == AppointmentStatus.Completed
               || a.Status == AppointmentStatus.Cancelled
               || a.Status == AppointmentStatus.NoShow))
           .ToListAsync();

        if (!oldAppointments.Any())
        {
            _logger.LogInformation("No old appointments to clean up");
            return;
        }

        foreach (var appointment in oldAppointments)
            repository.Delete(appointment);

        await repository.SaveChangesAsync();

        _logger.LogInformation("Cleaned up {Count} old appointments", oldAppointments.Count);
    }
}
